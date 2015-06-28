using CommonWin32;
using CommonWin32.API;
using CommonWin32.Monitors;
using CommonWin32.Rectangles;
using CommonWin32.Windows;
using ModernWPF.Converters;
using ModernWPF.Native;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;

namespace ModernWPF.Controls
{
    /// <summary>
    /// Used for actual processing since Chrome is freezable and don't want to keep states there.
    /// </summary>
    sealed class LegacyBorderManager : DependencyObject
    {
        public static LegacyBorderManager GetWorker(DependencyObject obj)
        {
            return (LegacyBorderManager)obj.GetValue(WorkerProperty);
        }

        public static void SetWorker(DependencyObject obj, LegacyBorderManager value)
        {
            obj.SetValue(WorkerProperty, value);
        }

        public static readonly DependencyProperty WorkerProperty =
            DependencyProperty.RegisterAttached("Worker", typeof(LegacyBorderManager), typeof(LegacyBorderManager), new PropertyMetadata(null, ChromeWorkerChanged));

        private static void ChromeWorkerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var window = d as Window;
            if (d != null)
            {
                var oldChrome = e.OldValue as LegacyBorderManager;
                var newChrome = e.NewValue as LegacyBorderManager;

                if (oldChrome != newChrome)
                {
                    if (oldChrome != null)
                    {
                        oldChrome.DetatchWindow();
                    }

                    if (newChrome != null)
                    {
                        newChrome.AttachWindow(window);
                    }
                }
            }
        }

        public void ChangeChrome(Chrome chrome)
        {
            Debug.WriteLine("ChromeWorker changing chrome.");
            if (_borderWindow != null)
                _borderWindow.UpdateChromeBindings(chrome);
        }

        private void AttachWindow(Window window)
        {
            Debug.WriteLine("ChromeWorker attached.");
            _contentWindow = window;
            _borderWindow = new LegacyBorderWindow(window);
            _contentWindow.Closed += _contentWindow_Closed;
            _contentWindow.ContentRendered += _contentWindow_ContentRendered;


            var hwnd = new WindowInteropHelper(_contentWindow).Handle;
            if (hwnd == IntPtr.Zero)
            {
                _contentWindow.SourceInitialized += window_SourceInitialized;
            }
            else
            {
                InitReal(hwnd);
            }
        }

        private void DetatchWindow()
        {
            Debug.WriteLine("ChromeWorker detatched");
            _resizeGrip = null;
            if (_borderWindow != null)
            {
                var toFoxus = _borderWindow.Owner;
                _borderWindow.Owner = null;
                _borderWindow.Close();
                _borderWindow = null;

                // hack to not let owner window move to background for some reason
                if (toFoxus != null) { toFoxus.Activate(); }
            }
            if (_contentWindow != null)
            {
                _contentWindow.Closed -= _contentWindow_Closed;
                _contentWindow.ContentRendered -= _contentWindow_ContentRendered;
                _contentWindow.SourceInitialized -= window_SourceInitialized;
                _contentWindow = null;
            }
        }


        Window _contentWindow;
        LegacyBorderWindow _borderWindow;
        ResizeGrip _resizeGrip;
        bool _hideOverride;
        bool _contentShown;

        #region window events

        void _contentWindow_Closed(object sender, EventArgs e)
        {
            LegacyBorderManager.SetWorker(_contentWindow, null);
        }

        void _contentWindow_ContentRendered(object sender, EventArgs e)
        {
            _resizeGrip = _contentWindow.FindChildInVisualTree<ResizeGrip>();
            // hack to make sure the border is shown after content is shown
            _contentShown = true;
            if (_borderWindow != null)
                _borderWindow.RepositionToContent(new WindowInteropHelper(_contentWindow).Handle, _hideOverride);
        }

        void window_SourceInitialized(object sender, EventArgs e)
        {
            var hwnd = new WindowInteropHelper(_contentWindow).Handle;
            InitReal(hwnd);
        }

        void InitReal(IntPtr hwnd)
        {
            HwndSource.FromHwnd(hwnd).AddHook(WndProc);
            UpdateFrame(hwnd);
        }

        #endregion

        #region win32 handling

        //private const int COLOR_WINDOW = 5;
        //[DllImport("user32.dll")]
        //static extern IntPtr GetSysColorBrush(int nIndex);
        static void UpdateFrame(IntPtr handle)
        {
            SetRegion(handle, 0, 0, true);

            // SWP_DRAWFRAME makes window bg really transparent (visible during resize) and not black
            User32.SetWindowPos(handle, IntPtr.Zero, 0, 0, 0, 0,
                SetWindowPosOptions.SWP_NOOWNERZORDER |
                SetWindowPosOptions.SWP_DRAWFRAME |
                SetWindowPosOptions.SWP_NOACTIVATE |
                SetWindowPosOptions.SWP_NOZORDER |
                SetWindowPosOptions.SWP_NOMOVE |
                SetWindowPosOptions.SWP_NOSIZE);

            //var result = User32.SetClassLong(handle, CommonWin32.WindowClasses.ClassLong.GCLP_HBRBACKGROUND, GetSysColorBrush(COLOR_WINDOW));
        }

        /// <summary>
        /// Handles Win32 window messages for this window.
        /// </summary>
        /// <param name="hwnd">The window handle.</param>
        /// <param name="msg">The message ID.</param>
        /// <param name="wParam">The message's wParam value.</param>
        /// <param name="lParam">The message's lParam value.</param>
        /// <param name="handled">A value that indicates whether the message was handled. Set the value to
        /// true if the message was handled; otherwise, false.</param>
        /// <returns></returns>
        IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            IntPtr retVal = IntPtr.Zero;
            if (!handled)
            {
                var wmsg = (WindowMessage)msg;
                //Debug.WriteLine(wmsg);
                switch (wmsg)
                {
                    case WindowMessage.WM_MOUSEHWHEEL:
                        // do our own horizontal wheel event
                        var element = Mouse.DirectlyOver;
                        if (element != null)
                        {
                            var delta = wParam.ToInt32() >> 16;
                            var arg = new MouseWheelEventArgs(InputManager.Current.PrimaryMouseDevice, Environment.TickCount, delta)
                            {
                                RoutedEvent = MouseEvents.PreviewMouseHWheelEvent
                            };
                            element.RaiseEvent(arg);
                            if (!arg.Handled)
                            {
                                arg.RoutedEvent = MouseEvents.MouseHWheelEvent;
                                arg.Handled = false;
                                element.RaiseEvent(arg);

                                handled = arg.Handled;
                            }
                        }
                        break;
                    //case WindowMessage.WM_SETTEXT:
                    //case WindowMessage.WM_SETICON:
                    //    var changed = User32Ex.ModifyStyle(hwnd, WindowStyles.WS_VISIBLE, WindowStyles.WS_OVERLAPPED);
                    //    retVal = User32.DefWindowProc(hwnd, (uint)msg, wParam, lParam);
                    //    if (changed) { User32Ex.ModifyStyle(hwnd, WindowStyles.WS_OVERLAPPED, WindowStyles.WS_VISIBLE); }
                    //    handled = true;
                    //    break;
                    case WindowMessage.WM_NCUAHDRAWCAPTION:
                    case WindowMessage.WM_NCUAHDRAWFRAME:
                        // undocumented stuff for non-dwm themes that will sometimes draw old control buttons
                        handled = true;
                        break;
                    case WindowMessage.WM_NCCALCSIZE:
                        //remove non-client borders completely
                        HandleNcCalcSize(hwnd, wParam, lParam);
                        handled = true;
                        break;
                    case WindowMessage.WM_NCPAINT:
                        // prevent non-dwm flicker
                        handled = !Dwmapi.IsCompositionEnabled;
                        break;
                    case WindowMessage.WM_NCACTIVATE:
                        // handled to prevent default non-client border from showing in classic mode
                        // wParam False means draw inactive title bar (which we do nothing).

                        retVal = HandleNcActivate(hwnd, msg, wParam, retVal);
                        handled = true;
                        break;
                    case WindowMessage.WM_NCHITTEST:
                        retVal = new IntPtr((int)HandleNcHitTest(lParam.ToPoint()));
                        handled = true;
                        break;
                    case WindowMessage.WM_NCRBUTTONDOWN:
                        switch ((NcHitTest)wParam.ToInt32())
                        {
                            case NcHitTest.HTCAPTION:
                            case NcHitTest.HTSYSMENU:
                                // display sys menu
                                User32.PostMessage(hwnd, (uint)WindowMessage.WM_POPUPSYSTEMMENU, IntPtr.Zero, lParam);
                                handled = true;
                                break;
                        }
                        break;
                    case WindowMessage.WM_WINDOWPOSCHANGED:
                        HandleWindowPosChanged(hwnd, lParam);
                        break;
                    case WindowMessage.WM_DWMCOMPOSITIONCHANGED:
                        SetRegion(hwnd, 0, 0, true);
                        break;
                    case WindowMessage.WM_ERASEBKGND:
                        // prevent more flickers?
                        handled = true;
                        break;
                }
            }
            return retVal;
        }

        private void HandleWindowPosChanged(IntPtr hwnd, IntPtr lParam)
        {
            var windowpos = (WINDOWPOS)Marshal.PtrToStructure(lParam, typeof(WINDOWPOS));
            //Debug.WriteLine("Chrome {0} windowpos flags {1}.", _borderWindow.Id, ClearUndefined(windowpos.flags));

            if ((windowpos.flags & SetWindowPosOptions.SWP_NOSIZE) != SetWindowPosOptions.SWP_NOSIZE)
            {
                SetRegion(hwnd, windowpos.cx, windowpos.cy, false);
            }

            if (_borderWindow != null)
            {
                // The override is for a window with owner and the owner window minimizes.
                // In this case the window is hidden with SWP_HIDEWINDOW but not actually minimized
                // so the code detects the show/hide flags here as the override
                if ((windowpos.flags & SetWindowPosOptions.SWP_HIDEWINDOW) == SetWindowPosOptions.SWP_HIDEWINDOW)
                {
                    _hideOverride = true;
                }
                if ((windowpos.flags & SetWindowPosOptions.SWP_SHOWWINDOW) == SetWindowPosOptions.SWP_SHOWWINDOW)
                {
                    _hideOverride = false;
                }
                if (_contentShown)
                {
                    _borderWindow.RepositionToContent(hwnd, _hideOverride);
                }
            }
        }

        private IntPtr HandleNcActivate(IntPtr hwnd, int msg, IntPtr wParam, IntPtr retVal)
        {
            //Debug.WriteLine(hwnd.ToInt64() + " wparam " + wParam.ToInt32());

            if (wParam == BasicValues.FALSE)
            {
                retVal = BasicValues.TRUE;
            }
            else
            {
                // Also skip default wndproc on maximized window to prevent non-dwm theme titlebar being drawn
                if (_contentWindow.WindowState != WindowState.Maximized ||
                    Dwmapi.IsCompositionEnabled)
                {
                    retVal = User32.DefWindowProc(hwnd, (uint)msg, wParam, new IntPtr(-1));
                }
            }
            return retVal;
        }


        static SetWindowPosOptions ClearUndefined(SetWindowPosOptions input)
        {
            SetWindowPosOptions retVal = 0;
            foreach (SetWindowPosOptions val in Enum.GetValues(typeof(SetWindowPosOptions)))
            {
                if ((input & val) == val)
                {
                    retVal |= val;
                }
            }
            return retVal;
        }

        static void SetRegion(IntPtr hwnd, int width, int height, bool force)
        {
            if (Dwmapi.IsCompositionEnabled)
            {
                //clear
                if (force)
                {
                    User32.SetWindowRgn(hwnd, IntPtr.Zero, User32.IsWindowVisible(hwnd));
                }
            }
            else
            {
                var wpl = default(WINDOWPLACEMENT);
                wpl.length = (uint)Marshal.SizeOf(typeof(WINDOWPLACEMENT));

                if (User32.GetWindowPlacement(hwnd, ref wpl))
                {
                    if (wpl.showCmd == ShowWindowOption.SW_MAXIMIZE)
                    {
                        //clear
                        User32.SetWindowRgn(hwnd, IntPtr.Zero, User32.IsWindowVisible(hwnd));
                    }
                    else
                    {
                        // always rectangle to prevent rounded corners for some themes
                        IntPtr rgn = IntPtr.Zero;
                        try
                        {
                            if (width == 0 || height == 0)
                            {
                                RECT r = default(RECT);
                                User32.GetWindowRect(hwnd, ref r);
                                width = r.Width;
                                height = r.Height;
                            }

                            rgn = Gdi32.CreateRectRgn(0, 0, width, height);
                            User32.SetWindowRgn(hwnd, rgn, User32.IsWindowVisible(hwnd));
                        }
                        finally
                        {
                            if (rgn != IntPtr.Zero)
                            {
                                Gdi32.DeleteObject(rgn);
                            }
                        }
                    }
                }
            }
        }

        static void HandleNcCalcSize(IntPtr hwnd, IntPtr wParam, IntPtr lParam)
        {
            if (wParam == BasicValues.TRUE)
            {
                var wpl = default(WINDOWPLACEMENT);
                wpl.length = (uint)Marshal.SizeOf(typeof(WINDOWPLACEMENT));

                if (User32.GetWindowPlacement(hwnd, ref wpl))
                {
                    // detect if maximizd and set to workspace remove padding
                    if (wpl.showCmd == ShowWindowOption.SW_MAXIMIZE)
                    {
                        // in multi-monitor case where app is minimized to a monitor on the right/bottom 
                        // the MonitorFromWindow will incorrectly return the leftmost monitor due to the minimized
                        // window being set to the far left, so this routine now uses the proposed rect to correctly
                        // identify the real nearest monitor to calc the nc size.

                        NCCALCSIZE_PARAMS para = (NCCALCSIZE_PARAMS)Marshal.PtrToStructure(lParam, typeof(NCCALCSIZE_PARAMS));

                        var windowRect = para.rectProposed;
                        IntPtr hMonitor = User32.MonitorFromRect(ref windowRect, MonitorOption.MONITOR_DEFAULTTONEAREST);// MonitorFromWindow(hWnd, 2);
                        if (hMonitor != IntPtr.Zero)
                        {
                            MONITORINFO lpmi = new MONITORINFO();
                            lpmi.cbSize = (uint)Marshal.SizeOf(typeof(MONITORINFO));
                            if (User32.GetMonitorInfo(hMonitor, ref lpmi))
                            {
                                var workArea = lpmi.rcWork;
                                User32Ex.AdjustForAutoHideTaskbar(hMonitor, ref workArea);
                                Debug.WriteLine("NCCalc original = {0}x{1} @ {2}x{3}, new ={4}x{5} @ {6}x{7}",
                                    para.rectProposed.Width, para.rectProposed.Height,
                                    para.rectProposed.left, para.rectProposed.top,
                                    workArea.Width, workArea.Height,
                                    workArea.left, workArea.top);
                                para.rectProposed = workArea;
                                Marshal.StructureToPtr(para, lParam, true);

                            }
                        }
                    }
                }
            }
        }

        NcHitTest HandleNcHitTest(Point screenPoint)
        {
            var windowPoint = _contentWindow.PointFromScreen(screenPoint);
            var capH = (double)WindowCaptionHeightConverter.Instance.Convert(Chrome.GetCaptionHeight(_contentWindow), typeof(Double), null, CultureInfo.CurrentCulture);
            //double capH = (windowCapH > -1 ? windowCapH : _contentWindow.ActualHeight);

            NcHitTest location = NcHitTest.HTCLIENT;
            var hitTest = _contentWindow.InputHitTest(windowPoint);

            if (hitTest != null && (windowPoint.Y <= capH || Chrome.GetIsCaption(hitTest)) &&
                !Chrome.GetIsHitTestVisible(hitTest))
            {
                location = NcHitTest.HTCAPTION;
                if (windowPoint.Y <= 40)
                {
                    if (_contentWindow.FlowDirection == System.Windows.FlowDirection.LeftToRight)
                    {
                        if (windowPoint.X <= 40)
                        {
                            location = NcHitTest.HTSYSMENU;
                        }
                    }
                    else if (windowPoint.X >= (_contentWindow.ActualWidth - 40))
                    {
                        location = NcHitTest.HTSYSMENU;
                    }
                }
            }

            if (_resizeGrip != null && _resizeGrip.Visibility == System.Windows.Visibility.Visible &&
                VisualTreeHelper.HitTest(_resizeGrip, _resizeGrip.PointFromScreen(screenPoint)) != null)
            {
                location = _resizeGrip.FlowDirection == System.Windows.FlowDirection.LeftToRight ?
                    NcHitTest.HTBOTTOMRIGHT : NcHitTest.HTBOTTOMLEFT;
            }

            //Debug.WriteLine(location);
            return location;
        }


        #endregion
    }
}
