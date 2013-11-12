using CommonWin32;
using CommonWin32.API;
using CommonWin32.Monitors;
using CommonWin32.Rectangles;
using CommonWin32.Windows;
using ModernWPF.Controls;
using ModernWPF.Native;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;

namespace ModernWPF
{
    // yes this is the same idea as the WindowChrome class in framework 4.5 but greatly improved for modern style.

    /// <summary>
    /// Attached property class for making a <see cref="Window"/> modern.
    /// </summary>
    public class Chrome : Freezable
    {
        #region DPs

        #region hit test attached dp

        /// <summary>
        /// Attached property to mark a UI element as hit-testable when in the window caption area.
        /// </summary>
        public static readonly DependencyProperty IsHitTestVisibleProperty =
            DependencyProperty.RegisterAttached("IsHitTestVisible", typeof(bool), typeof(Chrome),
            new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.Inherits));

        /// <summary>
        /// Gets the IsHitTestVisible property for the element.
        /// </summary>
        /// <param name="inputElement">The input element.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException"></exception>
        public static bool GetIsHitTestVisible(IInputElement inputElement)
        {
            if (inputElement == null) { return false; }

            DependencyObject obj2 = inputElement as DependencyObject;
            if (obj2 == null)
            {
                throw new ArgumentException("The element must be a DependencyObject", "inputElement");
            }
            return (bool)obj2.GetValue(IsHitTestVisibleProperty);
        }

        /// <summary>
        /// Sets the IsHitTestVisible property for the element.
        /// </summary>
        /// <param name="inputElement">The input element.</param>
        /// <param name="hitTestVisible">if set to <c>true</c> then the element is hit test visible in chrome.</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="System.ArgumentException"></exception>
        public static void SetIsHitTestVisible(IInputElement inputElement, bool hitTestVisible)
        {
            if (inputElement == null) { throw new ArgumentNullException("inputElement"); }

            DependencyObject obj2 = inputElement as DependencyObject;
            if (obj2 == null)
            {
                throw new ArgumentException("The element must be a DependencyObject", "inputElement");
            }
            obj2.SetValue(IsHitTestVisibleProperty, hitTestVisible);
        }


        #endregion

        #region chrome attached dp

        /// <summary>
        /// Gets the chrome.
        /// </summary>
        /// <param name="window">The window.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">window</exception>
        public static Chrome GetChrome(Window window)
        {
            if (window == null) { throw new ArgumentNullException("window"); }
            return (Chrome)window.GetValue(Chrome.ChromeProperty);
        }

        /// <summary>
        /// Sets the chrome.
        /// </summary>
        /// <param name="window">The window.</param>
        /// <param name="chrome">The chrome.</param>
        /// <exception cref="System.ArgumentNullException">window</exception>
        public static void SetChrome(Window window, Chrome chrome)
        {
            if (window == null) { throw new ArgumentNullException("window"); }
            window.SetValue(Chrome.ChromeProperty, chrome);
        }
        /// <summary>
        /// The modern chrome attached property.
        /// </summary>
        public static readonly DependencyProperty ChromeProperty =
            DependencyProperty.RegisterAttached("Chrome", typeof(Chrome), typeof(Chrome), new PropertyMetadata(null, ChromeChanged));

        private static void ChromeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (DesignerProperties.GetIsInDesignMode(d)) { return; }

            Window window = d as Window;
            if (window != null)
            {
                // don't care about old chrome since it has no state
                Chrome newChrome = e.NewValue as Chrome;
                if (newChrome == null)
                {
                    ChromeWorker.SetWorker(window, null);
                }
                else if (e.NewValue != e.OldValue)
                {
                    var worker = ChromeWorker.GetWorker(window);
                    if (worker == null)
                    {
                        worker = new ChromeWorker();
                        ChromeWorker.SetWorker(window, worker);
                    }
                    else
                    {
                        worker.ChangeChrome(newChrome);
                    }
                }
            }
        }

        #endregion

        #region in window appearance attached dp

        /// <summary>
        /// Gets the height of the window caption area.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns></returns>
        public static double GetCaptionHeight(DependencyObject obj)
        {
            return (double)obj.GetValue(CaptionHeightProperty);
        }

        /// <summary>
        /// Sets the height of the window caption area.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="value">The value.</param>
        public static void SetCaptionHeight(DependencyObject obj, double value)
        {
            obj.SetValue(CaptionHeightProperty, value);
        }

        /// <summary>
        /// The dependency property for CaptionHeight.
        /// </summary>
        public static readonly DependencyProperty CaptionHeightProperty =
            DependencyProperty.RegisterAttached("CaptionHeight", typeof(double), typeof(Chrome), new PropertyMetadata(32d));


        /// <summary>
        /// Gets the value indicating whether to show caption text on the window.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns></returns>
        public static bool GetShowCaptionText(DependencyObject obj)
        {
            return (bool)obj.GetValue(ShowCaptionTextProperty);
        }

        /// <summary>
        /// Gets the value indicating whether to show caption text on the window.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="value">if set to <c>true</c> then show window title.</param>
        public static void SetShowCaptionText(DependencyObject obj, bool value)
        {
            obj.SetValue(ShowCaptionTextProperty, value);
        }

        /// <summary>
        /// The dependency property for ShowCaptionText.
        /// </summary>
        public static readonly DependencyProperty ShowCaptionTextProperty =
            DependencyProperty.RegisterAttached("ShowCaptionText", typeof(bool), typeof(Chrome), new PropertyMetadata(true));


        /// <summary>
        /// Gets the value indicating whether to show caption icon on the window.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns></returns>
        public static bool GetShowCaptionIcon(DependencyObject obj)
        {
            return (bool)obj.GetValue(ShowCaptionIconProperty);
        }

        /// <summary>
        /// Sets the value indicating whether to show caption text on the window.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="value">if set to <c>true</c> [value].</param>
        public static void SetShowCaptionIcon(DependencyObject obj, bool value)
        {
            obj.SetValue(ShowCaptionIconProperty, value);
        }

        /// <summary>
        /// The dependency property for ShowCaptionIcon.
        /// </summary>
        public static readonly DependencyProperty ShowCaptionIconProperty =
            DependencyProperty.RegisterAttached("ShowCaptionIcon", typeof(bool), typeof(Chrome), new PropertyMetadata(true));



        /// <summary>
        /// Gets whether to show control boxes in ModernWindow style.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns></returns>
        public static bool GetShowControlBoxes(DependencyObject obj)
        {
            return (bool)obj.GetValue(ShowControlBoxesProperty);
        }

        /// <summary>
        /// Sets whether to show control boxes.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="value">if set to <c>true</c> then show the control boxes.</param>
        public static void SetShowControlBoxes(DependencyObject obj, bool value)
        {
            obj.SetValue(ShowControlBoxesProperty, value);
        }

        /// <summary>
        /// The dependency property for ShowControlBoxes.
        /// </summary>
        public static readonly DependencyProperty ShowControlBoxesProperty =
            DependencyProperty.RegisterAttached("ShowControlBoxes", typeof(bool), typeof(Chrome), new PropertyMetadata(true));



        #endregion

        #region chrome border dp

        /// <summary>
        /// Gets the resize border thickness.
        /// </summary>
        /// <value>
        /// The resize border thickness.
        /// </value>
        public Thickness ResizeBorderThickness
        {
            get { return (Thickness)GetValue(ResizeBorderThicknessProperty); }
        }

        /// <summary>
        /// The dependency property for <see cref="ResizeBorderThickness"/>.
        /// </summary>
        public static readonly DependencyProperty ResizeBorderThicknessProperty =
            DependencyProperty.Register("ResizeBorderThickness", typeof(Thickness), typeof(Chrome), new PropertyMetadata(new Thickness(8)));


        /// <summary>
        /// Gets or sets the active border brush.
        /// </summary>
        /// <value>
        /// The active border brush.
        /// </value>
        [Category("Appearance")]
        public Brush ActiveBorderBrush
        {
            get { return (Brush)GetValue(ActiveBorderBrushProperty); }
            set { SetValue(ActiveBorderBrushProperty, value); }
        }

        /// <summary>
        /// The dependency property for <see cref="ActiveBorderBrush"/>.
        /// </summary>
        public static readonly DependencyProperty ActiveBorderBrushProperty =
            DependencyProperty.Register("ActiveBorderBrush", typeof(Brush), typeof(Chrome), new PropertyMetadata(Brushes.DimGray));


        /// <summary>
        /// Gets or sets the inactive border brush.
        /// </summary>
        /// <value>
        /// The inactive border brush.
        /// </value>
        [Category("Appearance")]
        public Brush InactiveBorderBrush
        {
            get { return (Brush)GetValue(InactiveBorderBrushProperty); }
            set { SetValue(InactiveBorderBrushProperty, value); }
        }

        /// <summary>
        /// The dependency property for <see cref="InactiveBorderBrush"/>.
        /// </summary>
        public static readonly DependencyProperty InactiveBorderBrushProperty =
            DependencyProperty.Register("InactiveBorderBrush", typeof(Brush), typeof(Chrome), new PropertyMetadata(Brushes.LightGray));


        /// <summary>
        /// Gets or sets the active caption brush.
        /// </summary>
        /// <value>
        /// The active caption brush.
        /// </value>
        [Category("Appearance")]
        public Brush ActiveCaptionBrush
        {
            get { return (Brush)GetValue(ActiveCaptionBrushProperty); }
            set { SetValue(ActiveCaptionBrushProperty, value); }
        }

        /// <summary>
        /// The dependency property for <see cref="ActiveCaptionBrush"/>.
        /// </summary>
        public static readonly DependencyProperty ActiveCaptionBrushProperty =
            DependencyProperty.Register("ActiveCaptionBrush", typeof(Brush), typeof(Chrome), new PropertyMetadata(Brushes.Transparent));


        /// <summary>
        /// Gets or sets the inactive caption brush.
        /// </summary>
        /// <value>
        /// The inactive caption brush.
        /// </value>
        [Category("Appearance")]
        public Brush InactiveCaptionBrush
        {
            get { return (Brush)GetValue(InactiveCaptionBrushProperty); }
            set { SetValue(InactiveCaptionBrushProperty, value); }
        }

        /// <summary>
        /// The dependency property for <see cref="InactiveCaptionBrush"/>.
        /// </summary>
        public static readonly DependencyProperty InactiveCaptionBrushProperty =
            DependencyProperty.Register("InactiveCaptionBrush", typeof(Brush), typeof(Chrome), new PropertyMetadata(Brushes.Transparent));


        /// <summary>
        /// Gets or sets the active caption foreground.
        /// </summary>
        /// <value>
        /// The active caption foreground.
        /// </value>
        [Category("Appearance")]
        public Brush ActiveCaptionForeground
        {
            get { return (Brush)GetValue(ActiveCaptionForegroundProperty); }
            set { SetValue(ActiveCaptionForegroundProperty, value); }
        }

        /// <summary>
        /// The dependency property for <see cref="ActiveCaptionForeground"/>.
        /// </summary>
        public static readonly DependencyProperty ActiveCaptionForegroundProperty =
            DependencyProperty.Register("ActiveCaptionForeground", typeof(Brush), typeof(Chrome), new PropertyMetadata(Brushes.Black));


        /// <summary>
        /// Gets or sets the inactive caption foreground.
        /// </summary>
        /// <value>
        /// The inactive caption foreground.
        /// </value>
        [Category("Appearance")]
        public Brush InactiveCaptionForeground
        {
            get { return (Brush)GetValue(InactiveCaptionForegroundProperty); }
            set { SetValue(InactiveCaptionForegroundProperty, value); }
        }

        /// <summary>
        /// The dependency property for <see cref="InactiveCaptionForeground"/>.
        /// </summary>
        public static readonly DependencyProperty InactiveCaptionForegroundProperty =
            DependencyProperty.Register("InactiveCaptionForeground", typeof(Brush), typeof(Chrome), new PropertyMetadata(Brushes.DimGray));

        #endregion

        #endregion

        /// <summary>
        /// When implemented in a derived class, creates a new instance of the <see cref="T:System.Windows.Freezable" /> derived class.
        /// </summary>
        /// <returns>
        /// The new instance.
        /// </returns>
        protected override Freezable CreateInstanceCore()
        {
            return new Chrome();
        }


        /// <summary>
        /// Use this for actual processing since Chrome is freezable and don't want to keep states there.
        /// </summary>
        sealed class ChromeWorker : DependencyObject
        {
            public static ChromeWorker GetWorker(DependencyObject obj)
            {
                return (ChromeWorker)obj.GetValue(WorkerProperty);
            }

            public static void SetWorker(DependencyObject obj, ChromeWorker value)
            {
                obj.SetValue(WorkerProperty, value);
            }

            public static readonly DependencyProperty WorkerProperty =
                DependencyProperty.RegisterAttached("Worker", typeof(ChromeWorker), typeof(ChromeWorker), new PropertyMetadata(null, ChromeWorkerChanged));

            private static void ChromeWorkerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
            {
                var window = d as Window;
                if (d != null)
                {
                    var oldChrome = e.OldValue as ChromeWorker;
                    var newChrome = e.NewValue as ChromeWorker;

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
                _borderWindow.UpdateChromeBindings(chrome);
            }

            private void AttachWindow(Window window)
            {
                Debug.WriteLine("ChromeWorker attached.");
                _contentWindow = window;
                _borderWindow = new BorderWindow(window);
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
                if (_contentWindow != null)
                {
                    _contentWindow.Closed -= _contentWindow_Closed;
                    _contentWindow.ContentRendered -= _contentWindow_ContentRendered;
                    _contentWindow.SourceInitialized -= window_SourceInitialized;
                    _contentWindow = null;
                }
                if (_borderWindow != null)
                {
                    _borderWindow.Owner = null;
                    _borderWindow.Close();
                    _borderWindow = null;
                }
            }


            Window _contentWindow;
            BorderWindow _borderWindow;
            ResizeGrip _resizeGrip;
            bool _hideOverride;

            #region window events

            void _contentWindow_Closed(object sender, EventArgs e)
            {
                ChromeWorker.SetWorker(_contentWindow, null);
            }

            void _contentWindow_ContentRendered(object sender, EventArgs e)
            {
                _resizeGrip = _contentWindow.FindInVisualTree<ResizeGrip>();
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
            void UpdateFrame(IntPtr handle)
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
                        case WindowMessage.WM_SETTEXT:
                        case WindowMessage.WM_SETICON:
                            var changed = User32Ex.ModifyStyle(hwnd, WindowStyles.WS_VISIBLE, WindowStyles.WS_OVERLAPPED);
                            retVal = User32.DefWindowProc(hwnd, (uint)msg, wParam, lParam);
                            if (changed) { User32Ex.ModifyStyle(hwnd, WindowStyles.WS_OVERLAPPED, WindowStyles.WS_VISIBLE); }
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
                            // prevent default non-client border from showing in classic mode
                            if (wParam == BasicValues.FALSE)
                            {
                                retVal = BasicValues.TRUE;
                            }
                            else
                            {
                                retVal = User32.DefWindowProc(hwnd, (uint)msg, wParam, new IntPtr(-1));
                            }
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
                                _borderWindow.RepositionToContent(hwnd, _hideOverride);
                            }
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


            private SetWindowPosOptions ClearUndefined(SetWindowPosOptions input)
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

            private void SetRegion(IntPtr hwnd, int width, int height, bool force)
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

            private void HandleNcCalcSize(IntPtr hwnd, IntPtr wParam, IntPtr lParam)
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
                var windowCapH = GetCaptionHeight(_contentWindow);
                double capH = (windowCapH > -1 ? windowCapH : _contentWindow.ActualHeight);

                NcHitTest location = NcHitTest.HTCLIENT;

                if (windowPoint.Y <= capH)
                {
                    var hitTest = _contentWindow.InputHitTest(windowPoint);
                    if (hitTest != null && !GetIsHitTestVisible(hitTest))
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
}
