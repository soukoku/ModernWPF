﻿using CommonWin32.API;
using CommonWin32.MouseInput;
using CommonWin32.Window;
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

namespace ModernWPF.Controls
{
    /// <summary>
    /// Provides sizing border and drop shadow for a window.
    /// </summary>
    sealed class BorderWindow : Window
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline", Justification = "Only way to override style key DP.")]
        static BorderWindow()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BorderWindow), new FrameworkPropertyMetadata(typeof(BorderWindow)));
            WindowStyleProperty.OverrideMetadata(typeof(BorderWindow), new FrameworkPropertyMetadata(WindowStyle.None));
            ShowInTaskbarProperty.OverrideMetadata(typeof(BorderWindow), new FrameworkPropertyMetadata(false));
            AllowsTransparencyProperty.OverrideMetadata(typeof(BorderWindow), new FrameworkPropertyMetadata(true));
        }

        public bool IsContentActive
        {
            get { return (bool)GetValue(IsContentActiveProperty); }
            set { SetValue(IsContentActiveProperty, value); }
        }

        public static readonly DependencyProperty IsContentActiveProperty =
            DependencyProperty.Register("IsContentActive", typeof(bool), typeof(BorderWindow), new PropertyMetadata(false));




        public Brush InactiveBorderBrush
        {
            get { return (Brush)GetValue(InactiveBorderBrushProperty); }
            set { SetValue(InactiveBorderBrushProperty, value); }
        }

        // Using a DependencyProperty as the backing store for InactiveBorderBrush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty InactiveBorderBrushProperty =
            DependencyProperty.Register("InactiveBorderBrush", typeof(Brush), typeof(BorderWindow), new PropertyMetadata(null));



        Window _contentWindow;
        DispatcherTimer _showTimer;

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
                        this.Show();
                        if (_contentWindow != null)
                            _contentWindow.Activate();
                    }
                }
            }
            else
            {
                _showTimer.Stop();
                this.Hide();
            }
        }

        public BorderWindow(Chrome chrome, Window contentWindow)
        {
            _contentWindow = contentWindow;

            BindingTo("IsActive", contentWindow, IsContentActiveProperty);
            BindingTo(Chrome.ResizeBorderThicknessProperty.Name, chrome, BorderThicknessProperty);
            BindingTo(Chrome.ActiveBorderBrushProperty.Name, chrome, BorderBrushProperty);
            BindingTo(Chrome.InactiveBorderBrushProperty.Name, chrome, InactiveBorderBrushProperty);

            _showTimer = new DispatcherTimer();
            // magic # for windows animation duration
            _showTimer.Interval = TimeSpan.FromMilliseconds(250);
            _showTimer.Tick += (s, e) =>
            {
                _showTimer.Stop();
                this.Show();
                if (_contentWindow != null)
                    _contentWindow.Activate();
            };
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
            BindingOperations.ClearBinding(this, IsContentActiveProperty);
            BindingOperations.ClearBinding(this, BorderThicknessProperty);
            this.DataContext = null;
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

        public void RepositionToContent(IntPtr contentHwnd)
        {
            var thick = BorderThickness;
            var wpl = default(WINDOWPLACEMENT);
            wpl.length = (uint)Marshal.SizeOf(typeof(WINDOWPLACEMENT));

            if (User32.GetWindowPlacement(contentHwnd, ref wpl))
            {
                switch (wpl.showCmd)
                {
                    case ShowWindowOption.SW_SHOWNORMAL:
                        //Debug.WriteLine("Should reposn shadow");
                        // use GetWindowRect to work correctly with aero snap
                        var r = default(CommonWin32.Rectangle.RECT);
                        if (User32.GetWindowRect(contentHwnd, ref r))
                        {
                            User32.SetWindowPos(_hwnd, contentHwnd,
                                (int)(r.left - thick.Left),
                                (int)(r.top - thick.Top),
                                (int)(r.Width + thick.Left + thick.Right),
                                (int)(r.Height + thick.Top + thick.Bottom),
                            SetWindowPosOptions.SWP_NOACTIVATE);
                            ToggleVisible(true);
                            //Debug.WriteLine("reposned");
                        }
                        break;
                    case ShowWindowOption.SW_MAXIMIZE:
                    case ShowWindowOption.SW_MINIMIZE:
                    case ShowWindowOption.SW_SHOWMINIMIZED:
                        ToggleVisible(false);
                        //Debug.WriteLine("No shadow");
                        break;
                    default:
                        //Debug.WriteLine("Unknown showcmd " + wpl.showCmd);
                        break;
                }
            }


        }

        private void ApplyWin32Stuff(IntPtr hwnd)
        {
            // hide from alt tab
            var w = User32.GetWindowLong(hwnd, WindowLong.GWL_EXSTYLE).ToInt32();
            w |= (int)(WindowStylesEx.WS_EX_TOOLWINDOW | WindowStylesEx.WS_EX_NOACTIVATE);

            User32.SetWindowLong(hwnd, WindowLong.GWL_EXSTYLE, new IntPtr(w));

            // make resize more performant?
            User32.SetWindowPos(hwnd, IntPtr.Zero, 0, 0, 0, 0,
                SetWindowPosOptions.SWP_NOOWNERZORDER |
                SetWindowPosOptions.SWP_DRAWFRAME |
                SetWindowPosOptions.SWP_NOACTIVATE |
                SetWindowPosOptions.SWP_NOZORDER |
                SetWindowPosOptions.SWP_NOMOVE |
                SetWindowPosOptions.SWP_NOSIZE);

            // make the border window behind the content window.
            // commented out to test another method
            //var cwHwnd = new WindowInteropHelper(_contentWindow).Handle;
            //User32.SetWindowLong(cwHwnd, WindowLong.GWL_HWNDPARENT, hwnd);
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
                    case WindowMessage.WM_SETTEXT:
                    case WindowMessage.WM_SETICON:
                        var changed = User32Ex.ModifyStyle(hwnd, WindowStyles.WS_VISIBLE, WindowStyles.WS_OVERLAPPED);
                        retVal = User32.DefWindowProc(hwnd, (uint)msg, wParam, lParam);
                        if (changed) { User32Ex.ModifyStyle(hwnd, WindowStyles.WS_OVERLAPPED, WindowStyles.WS_VISIBLE); }
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
                        var low = 0xffff & lParam.ToInt32();
                        var hchit = (NcHitTest)low;
                        // in case of non-resizable window eat this msg
                        if (hchit == NcHitTest.HTCLIENT)
                        {
                            handled = true;
                            retVal = new IntPtr((int)MouseActivate.MA_NOACTIVATEANDEAT);
                        }
                        break;
                    case WindowMessage.WM_ERASEBKGND:
                        // prevent more flickers
                        handled = true;
                        break;
                }
            }
            return retVal;
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
            return NC_BORDER_MATRIX[row, col];
        }

        /// <summary>
        /// Matrix of the HT values to return for sizing border.
        /// </summary>
        static readonly NcHitTest[,] NC_BORDER_MATRIX = new NcHitTest[,]
        {
            { NcHitTest.HTTOPLEFT,    NcHitTest.HTTOP,     NcHitTest.HTTOPRIGHT},
            { NcHitTest.HTLEFT,       NcHitTest.HTCLIENT, NcHitTest.HTRIGHT       },
            { NcHitTest.HTBOTTOMLEFT, NcHitTest.HTBOTTOM, NcHitTest.HTBOTTOMRIGHT },
        };
    }
}
