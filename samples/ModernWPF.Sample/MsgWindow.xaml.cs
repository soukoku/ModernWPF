using CommonWin32.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ModernWPF.Sample
{
    /// <summary>
    /// Interaction logic for MsgWindow.xaml
    /// </summary>
    public partial class MsgWindow : Window
    {
        public MsgWindow()
        {
            InitializeComponent();
            this.SourceInitialized += MsgWindow_SourceInitialized;
        }

        void MsgWindow_SourceInitialized(object sender, EventArgs e)
        {
            var hwnd = new WindowInteropHelper(this).Handle;
            var src = HwndSource.FromHwnd(hwnd);
            src.AddHook(WndProc);

        }

        IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            IntPtr retVal = IntPtr.Zero;
            var wmsg = (WindowMessage)msg;
            msgBox.AppendText(string.Format("{0}\n", wmsg));
            msgBox.ScrollToEnd();
            return retVal;
        }
    }
}
