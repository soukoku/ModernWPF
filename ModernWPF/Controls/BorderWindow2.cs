//using CommonWin32.API;
//using CommonWin32.Windows;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Windows;
//using System.Windows.Data;
//using System.Windows.Interop;
//using System.Windows.Media;
//using System.Windows.Threading;

//namespace ModernWPF.Controls
//{
//    sealed class BorderWindow2 : DependencyObject, IDisposable
//    {
//        Window _contentWindow;
//        HwndSource _leftEdge;
//        HwndSource _topEdge;
//        HwndSource _rightEdge;
//        HwndSource _bottomEdge;
//        DispatcherTimer _showTimer;

//        const WindowStyles WS = WindowStyles.WS_POPUP | WindowStyles.WS_VISIBLE | WindowStyles.WS_CLIPSIBLINGS | WindowStyles.WS_CLIPCHILDREN;
//        const WindowStylesEx WSEX = WindowStylesEx.WS_EX_LAYERED | WindowStylesEx.WS_EX_TOOLWINDOW | WindowStylesEx.WS_EX_NOACTIVATE;

//        public BorderWindow2(Window contentWindow)
//        {
//            _contentWindow = contentWindow;
//            var chrome = Chrome.GetChrome(_contentWindow);

//            BindingTo("IsActive", contentWindow, IsContentActiveProperty);
//            UpdateChromeBindings(chrome);

//            _leftEdge = new HwndSource((int)ClassStyles.CS_NOCLOSE,
//                unchecked((int)WS),
//                (int)WSEX, 0, 0, "MWPF_GLOW_EDGE", IntPtr.Zero);
//            _leftEdge.AddHook(EdgeWndProc);
//            _topEdge = new HwndSource((int)ClassStyles.CS_NOCLOSE,
//                unchecked((int)WS),
//                (int)WSEX, 0, 0, "MWPF_GLOW_EDGE", IntPtr.Zero);
//            _topEdge.AddHook(EdgeWndProc);
//            _rightEdge = new HwndSource((int)ClassStyles.CS_NOCLOSE,
//                unchecked((int)WS),
//                (int)WSEX, 0, 0, "MWPF_GLOW_EDGE", IntPtr.Zero);
//            _rightEdge.AddHook(EdgeWndProc);
//            _bottomEdge = new HwndSource((int)ClassStyles.CS_NOCLOSE,
//                unchecked((int)WS),
//                (int)WSEX, 0, 0, "MWPF_GLOW_EDGE", IntPtr.Zero);
//            _bottomEdge.AddHook(EdgeWndProc);

//            _showTimer = new DispatcherTimer();
//            // magic # for windows animation duration
//            _showTimer.Interval = Animation.TypicalDuration;
//            _showTimer.Tick += (s, e) =>
//            {
//                _showTimer.Stop();
//                Show();
//                if (_contentWindow != null)
//                    _contentWindow.Activate();
//            };
//        }

//        private IntPtr EdgeWndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
//        {
//            handled = true;
//            return User32.DefWindowProc(hwnd, (uint)msg, wParam, lParam);

//            return IntPtr.Zero;
//        }

//        #region bindings

//        internal void UpdateChromeBindings(Chrome chrome)
//        {
//            BindingTo(Chrome.ResizeBorderThicknessProperty.Name, chrome, BorderThicknessProperty);
//            BindingTo(Chrome.ActiveBorderBrushProperty.Name, chrome, ActiveBorderBrushProperty);
//            BindingTo(Chrome.InactiveBorderBrushProperty.Name, chrome, InactiveBorderBrushProperty);
//        }

//        void BindingTo(string sourcePath, object source, DependencyProperty bindToProperty)
//        {
//            var abind = new Binding(sourcePath);
//            abind.Source = source;
//            abind.NotifyOnSourceUpdated = true;
//            BindingOperations.SetBinding(this, bindToProperty, abind);
//        }

//        void ClearBindings()
//        {
//            BindingOperations.ClearBinding(this, IsContentActiveProperty);
//            BindingOperations.ClearBinding(this, BorderThicknessProperty);
//            BindingOperations.ClearBinding(this, ActiveBorderBrushProperty);
//            BindingOperations.ClearBinding(this, InactiveBorderBrushProperty);
//        }

//        #endregion

//        #region properties

//        private Window _owner;

//        public Window Owner
//        {
//            get { return _owner; }
//            set { _owner = value; }
//        }


//        public bool IsContentActive
//        {
//            get { return (bool)GetValue(IsContentActiveProperty); }
//            set { SetValue(IsContentActiveProperty, value); }
//        }

//        public static readonly DependencyProperty IsContentActiveProperty =
//            DependencyProperty.Register("IsContentActive", typeof(bool), typeof(BorderWindow2), new PropertyMetadata(false));


//        public Brush ActiveBorderBrush
//        {
//            get { return (Brush)GetValue(ActiveBorderBrushProperty); }
//            set { SetValue(ActiveBorderBrushProperty, value); }
//        }

//        public static readonly DependencyProperty ActiveBorderBrushProperty =
//            DependencyProperty.Register("ActiveBorderBrush", typeof(Brush), typeof(BorderWindow2), new PropertyMetadata(Brushes.DimGray));


//        public Brush InactiveBorderBrush
//        {
//            get { return (Brush)GetValue(InactiveBorderBrushProperty); }
//            set { SetValue(InactiveBorderBrushProperty, value); }
//        }

//        public static readonly DependencyProperty InactiveBorderBrushProperty =
//            DependencyProperty.Register("InactiveBorderBrush", typeof(Brush), typeof(BorderWindow2), new PropertyMetadata(Brushes.LightGray));


//        public double BorderThickness
//        {
//            get { return (double)GetValue(BorderThicknessProperty); }
//            set { SetValue(BorderThicknessProperty, value); }
//        }

//        public static readonly DependencyProperty BorderThicknessProperty =
//            DependencyProperty.Register("BorderThickness", typeof(double), typeof(BorderWindow2), new PropertyMetadata(8d));



//        #endregion

//        #region IDisposable Members

//        public void Dispose()
//        {
//            _contentWindow = null;
//            if (_leftEdge != null)
//            {
//                _leftEdge.Dispose();
//                _leftEdge = null;
//            }
//            if (_topEdge != null)
//            {
//                _topEdge.Dispose();
//                _topEdge = null;
//            }
//            if (_rightEdge != null)
//            {
//                _rightEdge.Dispose();
//                _rightEdge = null;
//            }
//            if (_bottomEdge != null)
//            {
//                _bottomEdge.Dispose();
//                _bottomEdge = null;
//            }
//            ClearBindings();
//        }

//        #endregion

//        internal void RepositionToContent(IntPtr hWnd, bool hideOverride)
//        {
//            if (_leftEdge == null)
//            {
//            }

//        }

//        private void Show()
//        {
//        }

//        internal void Close()
//        {
//            Dispose();
//        }
//    }
//}
