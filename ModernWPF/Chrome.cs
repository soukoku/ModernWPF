using CommonWin32;
using CommonWin32.API;
using CommonWin32.Monitor;
using CommonWin32.Rectangle;
using CommonWin32.Window;
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
    public sealed class Chrome : Freezable, INotifyPropertyChanged
    {
        #region DPs

        #region HScroll attached dp

        /// <summary>
        /// Attached propert to allow horizontal scroll by mouse wheel if possible.
        /// </summary>
        public static readonly DependencyProperty HScrollOnWheelProperty =
            DependencyProperty.RegisterAttached
            (
                "HScrollOnWheel",
                typeof(bool),
                typeof(Chrome),
                new UIPropertyMetadata(false, OnHScrollOnWheelPropertyChanged)
            );
        /// <summary>
        /// Gets the HScrollOnWheel property for this object.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        public static bool GetHScrollOnWheel(DependencyObject obj)
        {
            return (bool)obj.GetValue(HScrollOnWheelProperty);
        }
        /// <summary>
        /// Sets the HScrollOnWheel property for this object.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="value">if set to <c>true</c> then h-scroll logic will be used if possible.</param>
        public static void SetHScrollOnWheel(DependencyObject obj, bool value)
        {
            obj.SetValue(HScrollOnWheelProperty, value);
        }

        private static void OnHScrollOnWheelPropertyChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            var scroller = dpo as ScrollViewer;
            if (dpo is ItemsControl)
            {
                scroller = ((ItemsControl)dpo).TryGetScrollerViewer();
            }

            if (scroller != null)
            {
                if ((bool)args.NewValue)
                {
                    scroller.PreviewMouseWheel += scroller_PreviewMouseWheel;
                }
                else
                {
                    scroller.PreviewMouseWheel -= scroller_PreviewMouseWheel;
                }
            }
        }

        static void scroller_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer scroller = sender as ScrollViewer;
            if (scroller != null && scroller.ComputedVerticalScrollBarVisibility != Visibility.Visible &&
                scroller.ComputedHorizontalScrollBarVisibility == Visibility.Visible)
            {
                if (e.Delta < 0)
                {
                    scroller.ScrollToHorizontalOffset(scroller.HorizontalOffset + 48);
                }
                else
                {
                    scroller.ScrollToHorizontalOffset(scroller.HorizontalOffset - 48);
                }
                e.Handled = true;
            }
        }

        #endregion

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
            DependencyProperty.RegisterAttached("Chrome", typeof(Chrome), typeof(Chrome), new PropertyMetadata(null, new PropertyChangedCallback(ChromeChanged)));

        private static void ChromeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (DesignerProperties.GetIsInDesignMode(d)) { return; }

            var window = d as Window;
            if (d != null)
            {
                var oldChrome = e.OldValue as Chrome;
                var newChrome = e.NewValue as Chrome;

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

        #endregion

        #region normal dp

        //private static void NotifyNormalDPChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{
        //    var chrome = d as Chrome;
        //    if (chrome != null)
        //    {
        //        chrome.RaisePropertyChanged(e.Property.Name);
        //    }
        //}

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



        /// <summary>
        /// Gets or sets the height of the window caption area.
        /// </summary>
        /// <value>
        /// The height of the caption.
        /// </value>
        public double CaptionHeight
        {
            get { return (double)GetValue(CaptionHeightProperty); }
            set
            {
                SetValue(CaptionHeightProperty, value);
                RaisePropertyChanged("CaptionHeightThickness");
            }
        }

        /// <summary>
        /// Gets the caption height as a bindable thickness.
        /// </summary>
        /// <value>
        /// The caption height thickness.
        /// </value>
        public Thickness CaptionHeightThickness
        {
            get
            {
                return new Thickness(0, CaptionHeight, 0, 0);
            }
        }

        /// <summary>
        /// The dependency property for <see cref="CaptionHeight"/>.
        /// </summary>
        public static readonly DependencyProperty CaptionHeightProperty =
            DependencyProperty.Register("CaptionHeight", typeof(double), typeof(Chrome), new PropertyMetadata(32d, (d, e) =>
            {
                ((Chrome)d).RaisePropertyChanged("CaptionHeightThickness");
            }));


        /// <summary>
        /// Gets or sets a value indicating whether to show caption text.
        /// </summary>
        /// <value>
        ///   <c>true</c> to show caption text; otherwise, <c>false</c>.
        /// </value>
        public bool ShowCaptionText
        {
            get { return (bool)GetValue(ShowCaptionTextProperty); }
            set { SetValue(ShowCaptionTextProperty, value); }
        }

        /// <summary>
        /// The dependency property for <see cref="ShowCaptionText"/>.
        /// </summary>
        public static readonly DependencyProperty ShowCaptionTextProperty =
            DependencyProperty.Register("ShowCaptionText", typeof(bool), typeof(Chrome), new PropertyMetadata(true));


        /// <summary>
        /// Gets or sets a value indicating whether to show caption icon.
        /// </summary>
        /// <value>
        ///   <c>true</c> to show caption icon; otherwise, <c>false</c>.
        /// </value>
        public bool ShowCaptionIcon
        {
            get { return (bool)GetValue(ShowCaptionIconProperty); }
            set { SetValue(ShowCaptionIconProperty, value); }
        }

        /// <summary>
        /// The dependency property for <see cref="ShowCaptionIcon"/>.
        /// </summary>
        public static readonly DependencyProperty ShowCaptionIconProperty =
            DependencyProperty.Register("ShowCaptionIcon", typeof(bool), typeof(Chrome), new PropertyMetadata(true));



        #endregion

        #endregion

        Window _contentWindow;
        BorderWindow _borderWindow;
        ResizeGrip _resizeGrip;

        #region init methods

        private void AttachWindow(Window window)
        {
            _contentWindow = window;
            _borderWindow = new BorderWindow(this, window);
            _contentWindow.Closed += _contentWindow_Closed;
            _contentWindow.ContentRendered += _contentWindow_ContentRendered;


            var hwnd = new WindowInteropHelper(_contentWindow).Handle;
            if (hwnd == IntPtr.Zero)
            {
                _contentWindow.SourceInitialized += window_SourceInitialized;
            }
            else
            {
                HwndSource.FromHwnd(hwnd).AddHook(WndProc);
                UpdateFrame(hwnd);
            }
        }

        private void DetatchWindow()
        {
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
                _borderWindow.Close();
                _borderWindow = null;
            }
        }

        void _contentWindow_Closed(object sender, EventArgs e)
        {
            DetatchWindow();
        }

        void _contentWindow_ContentRendered(object sender, EventArgs e)
        {
            _resizeGrip = _contentWindow.FindInVisualTree<ResizeGrip>();
        }

        void window_SourceInitialized(object sender, EventArgs e)
        {
            var hwnd = new WindowInteropHelper(_contentWindow).Handle;
            HwndSource.FromHwnd(hwnd).AddHook(WndProc);
            UpdateFrame(hwnd);
        }

        #endregion

        #region win32 handling

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
                        //User32.DefWindowProc(hwnd, (uint)msg, wParam, new IntPtr(-1));
                        if (wParam == BasicValues.FALSE)
                        {
                            retVal = BasicValues.TRUE;
                        }
                        handled = true;
                        break;
                    case WindowMessage.WM_SETTEXT:
                        retVal = User32.DefWindowProc(hwnd, (uint)msg, wParam, lParam);
                        handled = true;
                        break;
                    case WindowMessage.WM_NCHITTEST:
                        // new from http://stackoverflow.com/questions/7913325/win-api-in-c-get-hi-and-low-word-from-intptr
                        // to handle possible negative values from multi-monitor setup
                        int x = unchecked((short)lParam);
                        int y = unchecked((short)((uint)lParam >> 16));

                        retVal = new IntPtr((int)HandleNcHitTest(new Point(x, y)));
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
                        RepositionBorder(hwnd);

                        var windowpos = (WINDOWPOS)Marshal.PtrToStructure(lParam, typeof(WINDOWPOS));
                        if ((windowpos.flags & SetWindowPosOptions.SWP_NOSIZE) != SetWindowPosOptions.SWP_NOSIZE)
                        {
                            SetRegion(hwnd, windowpos.cx, windowpos.cy, false);
                        }
                        break;
                    case WindowMessage.WM_DWMCOMPOSITIONCHANGED:
                        SetRegion(hwnd, 0, 0, true);
                        break;
                    case WindowMessage.WM_ERASEBKGND:
                        // prevent more flickers
                        handled = true;
                        break;
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

        private void RepositionBorder(IntPtr hwnd)
        {
            if (_borderWindow != null)
            {
                _borderWindow.Owner = _contentWindow.Owner;

                var thick = _borderWindow.BorderThickness;
                var wpl = default(WINDOWPLACEMENT);
                wpl.length = (uint)Marshal.SizeOf(typeof(WINDOWPLACEMENT));

                if (User32.GetWindowPlacement(hwnd, ref wpl))
                {
                    switch (wpl.showCmd)
                    {
                        case ShowWindowOption.SW_SHOWNORMAL:
                            //Debug.WriteLine("Should reposn shadow");
                            // use GetWindowRect to work correctly with aero snap
                            var r = default(CommonWin32.Rectangle.RECT);
                            if (User32.GetWindowRect(hwnd, ref r))
                            {
                                _borderWindow.Left = r.left - thick.Left;
                                _borderWindow.Top = r.top - thick.Top;
                                _borderWindow.Width = r.Width + thick.Left + thick.Right;
                                _borderWindow.Height = r.Height + thick.Top + thick.Bottom;
                                _borderWindow.ToggleVisible(true);
                                //Debug.WriteLine("reposned");
                            }
                            break;
                        case ShowWindowOption.SW_MAXIMIZE:
                        case ShowWindowOption.SW_MINIMIZE:
                        case ShowWindowOption.SW_SHOWMINIMIZED:
                            _borderWindow.ToggleVisible(false);
                            //Debug.WriteLine("No shadow");
                            break;
                        default:
                            //Debug.WriteLine("Unknown showcmd " + wpl.showCmd);
                            break;
                    }
                }
            }
        }

        NcHitTest HandleNcHitTest(Point screenPoint)
        {
            var windowPoint = _contentWindow.PointFromScreen(screenPoint);
            double capH = (CaptionHeight > -1 ? CaptionHeight : (double)CaptionHeightProperty.DefaultMetadata.DefaultValue);

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

        /// <summary>
        /// When implemented in a derived class, creates a new instance of the <see cref="T:System.Windows.Freezable" /> derived class.
        /// </summary>
        /// <returns>
        /// The new instance.
        /// </returns>
        protected override Freezable CreateInstanceCore()
        {
            return (Freezable)new Chrome();
        }

        #region INotifyPropertyChanged Members

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        void RaisePropertyChanged(string property)
        {
            var hand = PropertyChanged;
            if (hand != null) { hand(this, new PropertyChangedEventArgs(property)); }
        }

        #endregion
    }
}
