using CommonWin32;
using CommonWin32.API;
using CommonWin32.MouseInput;
using CommonWin32.Windows;
using CommonWin32.WindowClasses;
using ModernWPF.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;
using System.Diagnostics;
using CommonWin32.Monitors;

namespace ModernWPF.Controls
{
    /// <summary>
    /// Provides sizing border and drop shadow for a window.
    /// </summary>
    class BorderWindow : Window
    {
        #region DPs

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline", Justification = "Only way to override style key DP.")]
        static BorderWindow()
        {
            //DefaultStyleKeyProperty.OverrideMetadata(typeof(BorderWindow), new FrameworkPropertyMetadata(typeof(BorderWindow)));
            WindowStyleProperty.OverrideMetadata(typeof(BorderWindow), new FrameworkPropertyMetadata(WindowStyle.None));
            ShowInTaskbarProperty.OverrideMetadata(typeof(BorderWindow), new FrameworkPropertyMetadata(false));
            AllowsTransparencyProperty.OverrideMetadata(typeof(BorderWindow), new FrameworkPropertyMetadata(true));
            ShowActivatedProperty.OverrideMetadata(typeof(BorderWindow), new FrameworkPropertyMetadata(false));
            // override to make border less visible initially for slow machines
            //WindowStateProperty.OverrideMetadata(typeof(BorderWindow), new FrameworkPropertyMetadata(WindowState.Minimized));
            WidthProperty.OverrideMetadata(typeof(BorderWindow), new FrameworkPropertyMetadata(1d));
            HeightProperty.OverrideMetadata(typeof(BorderWindow), new FrameworkPropertyMetadata(1d));
        }

        public bool IsContentActive
        {
            get { return (bool)GetValue(IsContentActiveProperty); }
            set { SetValue(IsContentActiveProperty, value); }
        }

        public static readonly DependencyProperty IsContentActiveProperty =
            DependencyProperty.Register("IsContentActive", typeof(bool), typeof(BorderWindow), new PropertyMetadata(false, HandleDPChanged));



        public Brush ActiveBorderBrush
        {
            get { return (Brush)GetValue(ActiveBorderBrushProperty); }
            set { SetValue(ActiveBorderBrushProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ActiveBorderBrush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ActiveBorderBrushProperty =
            DependencyProperty.Register("ActiveBorderBrush", typeof(Brush), typeof(BorderWindow), new PropertyMetadata(null, HandleDPChanged));



        public Brush InactiveBorderBrush
        {
            get { return (Brush)GetValue(InactiveBorderBrushProperty); }
            set { SetValue(InactiveBorderBrushProperty, value); }
        }

        // Using a DependencyProperty as the backing store for InactiveBorderBrush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty InactiveBorderBrushProperty =
            DependencyProperty.Register("InactiveBorderBrush", typeof(Brush), typeof(BorderWindow), new PropertyMetadata(null, HandleDPChanged));

        private static void HandleDPChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Window)d).InvalidateVisual();
        }



        #endregion

        Window _contentWindow;
        DispatcherTimer _showTimer;

        public BorderWindow(Window contentWindow)
        {
            // only works if set directly, no in override
            this.Background = Brushes.Transparent;

            _contentWindow = contentWindow;
            var chrome = Chrome.GetChrome(_contentWindow);

            BindingTo("IsActive", contentWindow, IsContentActiveProperty);
            UpdateChromeBindings(chrome);

            _showTimer = new DispatcherTimer();
            // magic # for windows animation duration
            // this is used to not show border before content window 
            // is fully restored as normal from min/max states
            _showTimer.Interval = Animation.TypicalDuration;
            _showTimer.Tick += (s, e) =>
            {
                ShowReal();
            };
        }

        internal void UpdateChromeBindings(Chrome chrome)
        {
            BindingTo(Chrome.ResizeBorderThicknessProperty.Name, chrome, BorderThicknessProperty);
            BindingTo(Chrome.ActiveBorderBrushProperty.Name, chrome, ActiveBorderBrushProperty);
            BindingTo(Chrome.InactiveBorderBrushProperty.Name, chrome, InactiveBorderBrushProperty);
        }

        private void BindingTo(string sourcePath, object source, DependencyProperty bindToProperty)
        {
            var abind = new Binding(sourcePath);
            abind.Source = source;
            abind.NotifyOnSourceUpdated = true;
            BindingOperations.SetBinding(this, bindToProperty, abind);
        }

        protected override void OnClosed(EventArgs e)
        {
            _contentWindow = null;
            BindingOperations.ClearAllBindings(this);
            //BindingOperations.ClearBinding(this, IsContentActiveProperty);
            //BindingOperations.ClearBinding(this, BorderThicknessProperty);
            //BindingOperations.ClearBinding(this, ActiveBorderBrushProperty);
            //BindingOperations.ClearBinding(this, InactiveBorderBrushProperty);
            base.OnClosed(e);
        }


        IntPtr _hwnd;
        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            _hwnd = new WindowInteropHelper(this).Handle;
            var src = HwndSource.FromHwnd(_hwnd);
            src.AddHook(WndProc);

            ApplyWin32Stuff(_hwnd);
        }

        static void ApplyWin32Stuff(IntPtr hwnd)
        {
            // hide from alt tab
            var w = User32.GetWindowLong(hwnd, WindowLong.GWL_EXSTYLE).ToInt32();
            w |= (int)(WindowStylesEx.WS_EX_TOOLWINDOW | WindowStylesEx.WS_EX_NOACTIVATE);

            User32.SetWindowLong(hwnd, WindowLong.GWL_EXSTYLE, new IntPtr(w));

            //// make resize more performant?
            //User32.SetWindowPos(hwnd, IntPtr.Zero, 0, 0, 0, 0,
            //    SetWindowPosOptions.SWP_NOOWNERZORDER |
            //    SetWindowPosOptions.SWP_DRAWFRAME |
            //    SetWindowPosOptions.SWP_NOACTIVATE |
            //    SetWindowPosOptions.SWP_NOZORDER |
            //    SetWindowPosOptions.SWP_NOMOVE |
            //    SetWindowPosOptions.SWP_NOSIZE);
        }

        /// <summary>
        /// Toggles the visibility of the border window while taking account into
        /// Windows animation settings.
        /// </summary>
        /// <param name="visible">if set to <c>true</c> the border will become visible.</param>
        void ToggleVisible(bool visible)
        {
            if (visible)
            {
                if (this.Visibility != System.Windows.Visibility.Visible)
                {
                    if (SystemParameters.MinimizeAnimation)
                    {
                        _showTimer.Start();
                    }
                    else
                    {
                        ShowReal();
                    }
                }
            }
            else
            {
                _showTimer.Stop();
                this.Hide();
            }
        }

        void ShowReal()
        {
            _showTimer.Stop();
            this.Show();
            if (this.WindowState != System.Windows.WindowState.Normal)
            {
                this.WindowState = System.Windows.WindowState.Normal;
            }
        }

        public void RepositionToContent(IntPtr contentHwnd, bool hideOverride)
        {
            if (hideOverride)
            {
                ToggleVisible(false);
            }
            else
            {
                var thick = TranslateToPixels(BorderThickness);
                var wpl = default(WINDOWPLACEMENT);
                wpl.length = (uint)Marshal.SizeOf(typeof(WINDOWPLACEMENT));

                if (User32.GetWindowPlacement(contentHwnd, ref wpl))
                {
                    //Debug.WriteLine("Chrome {0} placement cmd {1}, flag {2}.", _id, wpl.showCmd, wpl.flags);

                    switch (wpl.showCmd)
                    {
                        case ShowWindowOption.SW_SHOWNORMAL:
                            this.Owner = _contentWindow.Owner; // for working as dialog

                            //Debug.WriteLine("Should reposn shadow");
                            // use GetWindowRect to work correctly with aero snap
                            // since GetWindowPlacement doesn't change
                            var r = default(CommonWin32.Rectangles.RECT);
                            if (User32.GetWindowRect(contentHwnd, ref r))
                            {
                                // for working with other app windows
                                User32.SetWindowPos(_hwnd, contentHwnd, 0, 0, 0, 0,
                                    SetWindowPosOptions.SWP_NOACTIVATE | SetWindowPosOptions.SWP_NOSIZE | SetWindowPosOptions.SWP_NOMOVE);


                                this.Left = r.left - thick.Left;
                                this.Top = r.top - thick.Top;
                                this.Width = r.Width + thick.Left + thick.Right;
                                this.Height = r.Height + thick.Top + thick.Bottom;

                                ToggleVisible(true);
                                //Debug.WriteLine("reposned");
                            }
                            break;
                        case ShowWindowOption.SW_MAXIMIZE:
                        case ShowWindowOption.SW_MINIMIZE:
                        case ShowWindowOption.SW_SHOWMINIMIZED:
                            //this.Owner = null;
                            ToggleVisible(false);
                            //Debug.WriteLine("No shadow");
                            break;
                        default:
                            //Debug.WriteLine("Unknown showcmd " + wpl.showCmd);
                            break;
                    }
                }
            }
        }

        private Thickness TranslateToPixels(Thickness wpfThickness)
        {
            // translate wpf units to actual pixels for high-dpi scaling
            var source = PresentationSource.FromVisual(this);
            if (source != null)
            {
                var transform = source.CompositionTarget.TransformToDevice;
                if (!transform.IsIdentity)
                {
                    var xScale = transform.M11;
                    var yScale = transform.M22;

                    var left = wpfThickness.Left * xScale;
                    var top = wpfThickness.Top * yScale;
                    var right = wpfThickness.Right * xScale;
                    var bottom = wpfThickness.Bottom * yScale;

                    return new Thickness(left, top, right, bottom);
                }
            }
            return wpfThickness;
        }

        IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            IntPtr retVal = IntPtr.Zero;
            if (!handled)
            {
                var wmsg = (WindowMessage)msg;
                //Debug.WriteLine(wmsg);
                switch (wmsg)
                {
                    //case WindowMessage.WM_SETTEXT:
                    //case WindowMessage.WM_SETICON:
                    //    var changed = User32Ex.ModifyStyle(hwnd, WindowStyles.WS_VISIBLE, WindowStyles.WS_OVERLAPPED);
                    //    retVal = User32.DefWindowProc(hwnd, (uint)msg, wParam, lParam);
                    //    if (changed) { User32Ex.ModifyStyle(hwnd, WindowStyles.WS_OVERLAPPED, WindowStyles.WS_VISIBLE); }
                    //    handled = true;
                    //    break;
                    case WindowMessage.WM_NCUAHDRAWCAPTION:
                    case WindowMessage.WM_NCUAHDRAWFRAME:
                        // undocumented stuff for lame non-dwm themes that will sometimes draw old control buttons
                        handled = true;
                        break;
                    case WindowMessage.WM_NCHITTEST:
                        retVal = new IntPtr((int)HandleNcHitTest(lParam.ToPoint()));
                        handled = true;
                        break;
                    case WindowMessage.WM_NCRBUTTONDOWN:
                    case WindowMessage.WM_NCMBUTTONDOWN:
                    case WindowMessage.WM_NCRBUTTONDBLCLK:
                    case WindowMessage.WM_NCMBUTTONDBLCLK:
                    case WindowMessage.WM_RBUTTONDBLCLK:
                    case WindowMessage.WM_MBUTTONDBLCLK:
                    case WindowMessage.WM_LBUTTONDBLCLK:
                        handled = true;
                        if (_contentWindow != null)
                            _contentWindow.Activate();
                        break;
                    case WindowMessage.WM_NCLBUTTONDOWN:
                    case WindowMessage.WM_NCLBUTTONDBLCLK:
                        Debug.WriteLine("Got border LButton down");
                        handled = true;
                        // pass resizer msg to the content window instead
                        if (_contentWindow != null)
                        {
                            _contentWindow.Activate();
                            var cwHwnd = new WindowInteropHelper(_contentWindow).Handle;
                            User32.SendMessage(cwHwnd, (uint)msg, wParam, IntPtr.Zero);
                        }
                        break;
                    case WindowMessage.WM_MOUSEACTIVATE:
                        var lowword = 0xffff & lParam.ToInt32();
                        var hchit = (NcHitTest)lowword;

                        // in case of non-resizable window eat this msg
                        if (hchit == NcHitTest.HTCLIENT)
                        {
                            retVal = new IntPtr((int)MouseActivate.MA_NOACTIVATEANDEAT);
                        }
                        else
                        {
                            retVal = new IntPtr((int)MouseActivate.MA_NOACTIVATE);
                        }
                        handled = true;
                        break;
                    case WindowMessage.WM_ERASEBKGND:
                        // prevent more flickers
                        handled = true;
                        break;
                    case WindowMessage.WM_GETMINMAXINFO:
                        HandleMinMaxInfo(lParam);
                        break;
                }
            }
            return retVal;
        }

        private void HandleMinMaxInfo(IntPtr lParam)
        {
            // overridden so max size = normal max + resize border (for when resizing content window to max size without maximizing)
            var thick = TranslateToPixels(BorderThickness);

            MINMAXINFO para = (MINMAXINFO)Marshal.PtrToStructure(lParam, typeof(MINMAXINFO));
            var orig = para.ptMaxTrackSize;
            orig.x += (int)(thick.Left + thick.Right);
            orig.y += (int)(thick.Top + thick.Bottom);
            para.ptMaxTrackSize = orig;
            Marshal.StructureToPtr(para, lParam, true);
        }

        NcHitTest HandleNcHitTest(Point screenPoint)
        {
            var windowPoint = this.PointFromScreen(screenPoint);

            NcHitTest hit = NcHitTest.HTCLIENT;
            if (_contentWindow != null &&
                (_contentWindow.ResizeMode == System.Windows.ResizeMode.CanResize ||
                _contentWindow.ResizeMode == System.Windows.ResizeMode.CanResizeWithGrip))
            {
                var chrome = _contentWindow.GetValue(Chrome.ChromeProperty) as Chrome;
                hit = NcBorderHitTest(chrome.ResizeBorderThickness, windowPoint);
            }
            //Debug.WriteLine(hit);
            return hit;
        }

        NcHitTest NcBorderHitTest(Thickness frame, Point windowPoint)
        {
            // Determine if hit test is for resizing, default middle (1,1).
            int row = 1;
            int col = 1;

            double top = frame.Top;
            double bottom = this.ActualHeight - frame.Bottom;
            double left = frame.Left;
            double right = this.ActualWidth - frame.Right;

            // Determine if the point is at the top or bottom of the window.
            if (windowPoint.Y <= top)
            {
                row = 0;  // top
            }
            else if (windowPoint.Y >= bottom)
            {
                row = 2; // bottom
            }

            // left-right test
            if (windowPoint.X <= left)
            {
                col = 0; // left side
            }
            else if (windowPoint.X >= right)
            {
                col = 2; // right side
            }
            return NC_BORDER_MATRIX[row][col];
        }

        /// <summary>
        /// Matrix of the HT values to return for sizing border.
        /// </summary>
        static readonly NcHitTest[][] NC_BORDER_MATRIX = 
        {
            new []{ NcHitTest.HTTOPLEFT,    NcHitTest.HTTOP,    NcHitTest.HTTOPRIGHT },
            new []{ NcHitTest.HTLEFT,       NcHitTest.HTCLIENT, NcHitTest.HTRIGHT     },
            new []{ NcHitTest.HTBOTTOMLEFT, NcHitTest.HTBOTTOM, NcHitTest.HTBOTTOMRIGHT },
        };


        #region border painter



        protected override void OnRender(DrawingContext ctx)
        {
            base.OnRender(ctx);

            // only solid brush is supported now
            var brush = (IsContentActive ? ActiveBorderBrush : InactiveBorderBrush) as SolidColorBrush;
            if (brush != null)
            {
                Rect rClient = new Rect(0, 0, this.ActualWidth, this.ActualHeight);

                var thick = BorderThickness;
                var clientW = rClient.Width - thick.Right - thick.Left;// -1;
                var clientH = rClient.Height - thick.Top - thick.Bottom;// -1;

                if (clientW > 1 && clientH > 1)
                {
                    rClient.X += thick.Left;
                    rClient.Y += thick.Top;
                    rClient.Width = clientW;
                    rClient.Height = clientH;

                    var rTop = new Rect(rClient.Left, 0, rClient.Width, thick.Top);
                    var rTopLeft = new Rect(0, 0, thick.Left, thick.Top);
                    var rTopRight = new Rect(rClient.Right, 0, thick.Right, thick.Top);

                    var rBottom = new Rect(rClient.Left, rClient.Bottom, rClient.Width, thick.Bottom);
                    var rBottomLeft = new Rect(0, rClient.Bottom, thick.Left, thick.Bottom);
                    var rBottomRight = new Rect(rClient.Right, rClient.Bottom, thick.Right, thick.Bottom);

                    var rLeft = new Rect(0, rClient.Top, thick.Left, rClient.Height);
                    var rRight = new Rect(rClient.Right, rClient.Top, thick.Right, rClient.Height);

                    var brushes = GetShadowBrushes(brush.Color, (thick.Top + thick.Bottom + thick.Right + thick.Left) / 4);
                    ctx.DrawRectangle(brushes[(int)BorderSide.TopLeft], null, rTopLeft);
                    ctx.DrawRectangle(brushes[(int)BorderSide.TopRight], null, rTopRight);
                    ctx.DrawRectangle(brushes[(int)BorderSide.Top], null, rTop);
                    ctx.DrawRectangle(brushes[(int)BorderSide.BottomLeft], null, rBottomLeft);
                    ctx.DrawRectangle(brushes[(int)BorderSide.BottomRight], null, rBottomRight);
                    ctx.DrawRectangle(brushes[(int)BorderSide.Bottom], null, rBottom);
                    ctx.DrawRectangle(brushes[(int)BorderSide.Left], null, rLeft);
                    ctx.DrawRectangle(brushes[(int)BorderSide.Right], null, rRight);


                    Pen borderPen = new Pen(brush, 1);

                    // from http://wpftutorial.net/DrawOnPhysicalDevicePixels.html
                    double halfPenWidth = borderPen.Thickness / 2;

                    // Create a guidelines set
                    GuidelineSet guidelines = new GuidelineSet();
                    guidelines.GuidelinesX.Add(rClient.Left + halfPenWidth);
                    guidelines.GuidelinesX.Add(rClient.Right + halfPenWidth);
                    guidelines.GuidelinesY.Add(rClient.Top + halfPenWidth);
                    guidelines.GuidelinesY.Add(rClient.Bottom + halfPenWidth);

                    ctx.PushGuidelineSet(guidelines);

                    rClient.X -= 1;
                    rClient.Y -= 1;
                    rClient.Width += 1;
                    rClient.Height += 1;
                    ctx.DrawRectangle(null, borderPen, rClient);
                    //ctx.DrawRectangle(null, new Pen(Brushes.Red, 1), rect);
                }
            }
        }


        static Brush[] GetShadowBrushes(Color color, double pad)
        {
            var brushes = new Brush[(int)BorderSide.Last];
            var stops = CreateStops(color);

            var top = brushes[(int)BorderSide.Top] = new LinearGradientBrush(stops, new Point(0.5, 1), new Point(0.5, 0.1));
            top.Freeze();
            var bottom = brushes[(int)BorderSide.Bottom] = new LinearGradientBrush(stops, new Point(0.5, 0), new Point(0.5, 1.1));
            bottom.Freeze();
            var left = brushes[(int)BorderSide.Left] = new LinearGradientBrush(stops, new Point(1, 0.5), new Point(0.1, 0.5));
            left.Freeze();
            var right = brushes[(int)BorderSide.Right] = new LinearGradientBrush(stops, new Point(0, 0.5), new Point(1, 0.5));
            right.Freeze();


            var topLeft = brushes[(int)BorderSide.TopLeft] = new RadialGradientBrush(stops)
            {
                RadiusX = .6,
                RadiusY = .6,
                Center = new Point(1, 1),
                GradientOrigin = new Point(1, 1),
            };
            topLeft.Freeze();

            var topRight = brushes[(int)BorderSide.TopRight] = new RadialGradientBrush(stops)
            {
                RadiusX = .7,
                RadiusY = .7,
                Center = new Point(0, 1),
                GradientOrigin = new Point(0, 1),
            };
            topRight.Freeze();

            var bottomLeft = brushes[(int)BorderSide.BottomLeft] = new RadialGradientBrush(stops)
            {
                RadiusX = .8,
                RadiusY = .8,
                Center = new Point(1, 0),
                GradientOrigin = new Point(1, 0),
            };
            bottomLeft.Freeze();

            var bottomRight = brushes[(int)BorderSide.BottomRight] = new RadialGradientBrush(stops)
            {
                RadiusX = .9,
                RadiusY = .9,
                Center = new Point(0, 0),
                GradientOrigin = new Point(0, 0),
            };
            bottomRight.Freeze();


            return brushes;
        }

        static GradientStopCollection CreateStops(Color c)
        {
            GradientStopCollection stops = new GradientStopCollection();
            Color stopColor = c;

            stopColor.A = (byte)(.45 * c.A);
            stops.Add(new GradientStop(stopColor, 0));

            stopColor.A = (byte)(.40 * c.A);
            stops.Add(new GradientStop(stopColor, .2));

            stopColor.A = (byte)(.25 * c.A);
            stops.Add(new GradientStop(stopColor, .45));

            stopColor.A = (byte)(.10 * c.A);
            stops.Add(new GradientStop(stopColor, .7));

            stopColor.A = (byte)(.05 * c.A);
            stops.Add(new GradientStop(stopColor, .85));

            stopColor.A = 0;
            stops.Add(new GradientStop(stopColor, 1));

            stops.Freeze();

            return stops;
        }

        enum BorderSide
        {
            TopLeft,
            Top,
            TopRight,
            Left,
            Right,
            BottomLeft,
            Bottom,
            BottomRight,
            Last,
        }


        #endregion
    }
}
