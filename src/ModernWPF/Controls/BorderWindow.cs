using CommonWin32;
using CommonWin32.API;
using CommonWin32.MouseInput;
using CommonWin32.Rectangles;
using CommonWin32.WindowClasses;
using CommonWin32.Windows;
using ModernWPF.Converters;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Shapes;

namespace ModernWPF.Controls
{
    /// <summary>
    /// Provides a single side of the resize/glow border window.
    /// </summary>
    class BorderWindow : Window
    {
        static BorderWindow()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BorderWindow), new FrameworkPropertyMetadata(typeof(BorderWindow)));
            WindowStyleProperty.OverrideMetadata(typeof(BorderWindow), new FrameworkPropertyMetadata(WindowStyle.None));
            ShowInTaskbarProperty.OverrideMetadata(typeof(BorderWindow), new FrameworkPropertyMetadata(false));
            AllowsTransparencyProperty.OverrideMetadata(typeof(BorderWindow), new FrameworkPropertyMetadata(true));
            ShowActivatedProperty.OverrideMetadata(typeof(BorderWindow), new FrameworkPropertyMetadata(false));
            ResizeModeProperty.OverrideMetadata(typeof(BorderWindow), new FrameworkPropertyMetadata(ResizeMode.NoResize));
            BorderBrushProperty.OverrideMetadata(typeof(BorderWindow), new FrameworkPropertyMetadata(Brushes.DimGray));
            // override to make border less visible initially for slow machines
            WidthProperty.OverrideMetadata(typeof(BorderWindow), new FrameworkPropertyMetadata(1d));
            HeightProperty.OverrideMetadata(typeof(BorderWindow), new FrameworkPropertyMetadata(1d));
        }

        #region DPs

        public BorderSide Side
        {
            get { return (BorderSide)GetValue(SideProperty); }
            set { SetValue(SideProperty, value); }
        }

        public static readonly DependencyProperty SideProperty =
            DependencyProperty.Register("Side", typeof(BorderSide), typeof(BorderWindow), new FrameworkPropertyMetadata(BorderSide.Left));



        public bool IsContentActive
        {
            get { return (bool)GetValue(IsContentActiveProperty); }
            set { SetValue(IsContentActiveProperty, value); }
        }

        public static readonly DependencyProperty IsContentActiveProperty =
            DependencyProperty.Register("IsContentActive", typeof(bool), typeof(BorderWindow), new FrameworkPropertyMetadata(false));



        public double BorderLength
        {
            get { return (double)GetValue(BorderLengthProperty); }
            set { SetValue(BorderLengthProperty, value); }
        }

        public static readonly DependencyProperty BorderLengthProperty =
            DependencyProperty.Register("BorderLength", typeof(double), typeof(BorderWindow), new FrameworkPropertyMetadata(1d));

        public double PadSize
        {
            get { return (double)GetValue(PadSizeProperty); }
            set { SetValue(PadSizeProperty, value); }
        }

        public static readonly DependencyProperty PadSizeProperty =
            DependencyProperty.Register("PadSize", typeof(double), typeof(BorderWindow), new FrameworkPropertyMetadata(8d));



        public Brush InactiveBorderBrush
        {
            get { return (Brush)GetValue(InactiveBorderBrushProperty); }
            set { SetValue(InactiveBorderBrushProperty, value); }
        }

        public static readonly DependencyProperty InactiveBorderBrushProperty =
            DependencyProperty.Register("InactiveBorderBrush", typeof(Brush), typeof(BorderWindow), new FrameworkPropertyMetadata(Brushes.Silver));




        #endregion


        IntPtr _hwnd;
        BorderManager _manager;
        public BorderWindow(BorderManager manager)
        {
            // only works if set directly, no in override
            this.Background = Brushes.Transparent;

            _manager = manager;

            BindingTo(IsActiveProperty.Name, _manager.ContentWindow, IsContentActiveProperty);
            UpdateChromeBindings(Chrome.GetChrome(_manager.ContentWindow));
        }

        internal void UpdateChromeBindings(Chrome chrome)
        {
            BindingTo(Chrome.ResizeBorderThicknessProperty.Name, chrome, PadSizeProperty, ThicknessToDoubleConverter.Instance, Side);
            BindingTo(Chrome.ActiveBorderBrushProperty.Name, chrome, BorderBrushProperty);
            BindingTo(Chrome.InactiveBorderBrushProperty.Name, chrome, InactiveBorderBrushProperty);
        }

        private void BindingTo(string sourcePath, object source, DependencyProperty bindToProperty, IValueConverter converter = null, object converterParameter = null)
        {
            var bind = new Binding(sourcePath);
            bind.Source = source;
            bind.NotifyOnSourceUpdated = true;
            bind.Converter = converter;
            bind.ConverterParameter = converterParameter;
            BindingOperations.SetBinding(this, bindToProperty, bind);
        }

        protected override void OnClosed(EventArgs e)
        {
            BindingOperations.ClearAllBindings(this);
            base.OnClosed(e);
        }

        internal void UpdatePosn(double left, double top, double width, double height)
        {
            //User32.SetWindowPos(_hwnd, _manager.hWndContent, left, top, width, height, SetWindowPosOptions.SWP_NOACTIVATE);
            this.Left = left;
            this.Top = top;
            this.Width = width;
            this.Height = height;
            var pad = 2 * PadSize;
            switch (Side)
            {
                case BorderSide.Left:
                case BorderSide.Right:
                    BorderLength = Math.Max(0, height - pad);
                    break;
                case BorderSide.Top:
                case BorderSide.Bottom:
                    BorderLength = Math.Max(0, width - pad);
                    break;
            }

            //Debug.WriteLine("Side {0} W={1}, actual W={2}", Side, Width, ActualWidth);
        }
        
        internal void ShowNoActivate()
        {
            Show();
            User32.SetWindowPos(_hwnd, _manager.hWndContent, 0, 0, 0, 0, 
                SetWindowPosOptions.SWP_NOMOVE | SetWindowPosOptions.SWP_NOSIZE | SetWindowPosOptions.SWP_NOACTIVATE);
        }


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
            var wsEx = User32.GetWindowLong(hwnd, WindowLong.GWL_EXSTYLE).ToInt32();
            wsEx |= (int)(WindowStylesEx.WS_EX_TOOLWINDOW | WindowStylesEx.WS_EX_NOACTIVATE);

            User32.SetWindowLong(hwnd, WindowLong.GWL_EXSTYLE, new IntPtr(wsEx));


            var ws = (uint)User32.GetWindowLong(hwnd, WindowLong.GWL_STYLE);
            // remove these to allow nchittest even when ResizeMode is NoResize
            ws &= ~(uint)(WindowStyles.WS_SYSMENU | WindowStyles.WS_OVERLAPPED);
            // todo: fix in 32bit world
            //ws |= (uint)(WindowStyles.WS_POPUP);

            User32.SetWindowLong(hwnd, WindowLong.GWL_STYLE, new IntPtr(ws));

            //// make resize more performant?
            //User32.SetWindowPos(hwnd, IntPtr.Zero, 0, 0, 0, 0,
            //    SetWindowPosOptions.SWP_NOOWNERZORDER |
            //    SetWindowPosOptions.SWP_DRAWFRAME |
            //    SetWindowPosOptions.SWP_NOACTIVATE |
            //    SetWindowPosOptions.SWP_NOZORDER |
            //    SetWindowPosOptions.SWP_NOMOVE |
            //    SetWindowPosOptions.SWP_NOSIZE);
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
                    case WindowMessage.WM_NCCALCSIZE:
                        handled = true;
                        break;
                    case WindowMessage.WM_NCHITTEST:
                        NcHitTest res = NcHitTest.HTBORDER;
                        if (_manager.ContentWindow.ResizeMode == ResizeMode.CanResizeWithGrip ||
                            _manager.ContentWindow.ResizeMode == ResizeMode.CanResize)
                        {
                            var pt = PointFromScreen(lParam.ToPoint());
                            int diagSize = (int)(2 * PadSize);
                            switch (Side)
                            {
                                case BorderSide.Left:
                                    if (pt.Y <= diagSize) { res = NcHitTest.HTTOPLEFT; }
                                    else if (pt.Y >= Height - diagSize) { res = NcHitTest.HTTOPLEFT; }
                                    else { res = NcHitTest.HTLEFT; }
                                    break;
                                case BorderSide.Top:
                                    if (pt.X <= diagSize) { res = NcHitTest.HTTOPLEFT; }
                                    else if (pt.X >= Width - diagSize) { res = NcHitTest.HTTOPRIGHT; }
                                    else { res = NcHitTest.HTTOP; }
                                    break;
                                case BorderSide.Right:
                                    if (pt.Y <= diagSize) { res = NcHitTest.HTTOPRIGHT; }
                                    else if (pt.Y >= Height - diagSize) { res = NcHitTest.HTBOTTOMRIGHT; }
                                    else { res = NcHitTest.HTRIGHT; }
                                    break;
                                case BorderSide.Bottom:
                                    if (pt.X <= diagSize) { res = NcHitTest.HTTOPRIGHT; }
                                    else if (pt.X >= Width - diagSize) { res = NcHitTest.HTBOTTOMRIGHT; }
                                    else { res = NcHitTest.HTBOTTOM; }
                                    break;
                            }
                        }
                        retVal = new IntPtr((int)res);
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
                        User32.SetForegroundWindow(_manager.hWndContent);
                        break;
                    case WindowMessage.WM_NCLBUTTONDOWN:
                    case WindowMessage.WM_NCLBUTTONDBLCLK:
                        handled = true;
                        // pass resizer msg to the content window instead
                        User32.SetForegroundWindow(_manager.hWndContent);
                        User32.SendMessage(_manager.hWndContent, (uint)msg, wParam, lParam);
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
                    case WindowMessage.WM_GETMINMAXINFO:
                        // overridden so max size = normal max + resize border (for when content window is max size without maximizing)
                        var thick = 2 * (int)PadSize;

                        MINMAXINFO para = (MINMAXINFO)Marshal.PtrToStructure(lParam, typeof(MINMAXINFO));
                        var orig = para.ptMaxTrackSize;
                        orig.x += thick;
                        orig.y += thick;
                        para.ptMaxTrackSize = orig;
                        Marshal.StructureToPtr(para, lParam, true);
                        break;
                }
            }
            return retVal;
        }

    }
}
