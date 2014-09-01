using GalaSoft.MvvmLight.Messaging;
using ModernWPF.Controls;
using ModernWPF.Messages;
using ModernWPF.Sample.VM;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ModernWPF.Sample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                Messenger.Default.Register<DialogMessage>(this, m => { if (m.Sender == this) { this.HandleDialogMessageModern(m); } });
                Messenger.Default.Register<ChooseFileMessage>(this, m => { if (m.Sender == this) { this.HandleChooseFile(m); } });
                Messenger.Default.Register<ChooseFolderMessage>(this, m => { if (m.Sender == this) { this.HandleChooseFolder(m); } });
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            Messenger.Default.Unregister(this);
            base.OnClosed(e);
        }

        private void btnRtl_Click(object sender, RoutedEventArgs e)
        {
            if (FlowDirection == System.Windows.FlowDirection.LeftToRight)
            {
                FlowDirection = System.Windows.FlowDirection.RightToLeft;
            }
            else
            {
                FlowDirection = System.Windows.FlowDirection.LeftToRight;
            }
        }

        private void btnWindow_Click(object sender, RoutedEventArgs e)
        {
            new MainWindow { Owner = this }.Show();
        }

        private void btnDialog_Click(object sender, RoutedEventArgs e)
        {
            new DialogWindow { Owner = this }.ShowDialog();
        }

        private void btnDialog2_Click(object sender, RoutedEventArgs e)
        {
            bool? lastResult = null;
            for (int i = 0; i < 3; )
            {
                var diag = new InWindowDialog();
                diag.Message = string.Format("This is modal dialog {0}/3 with last result = {1}, close it until the stack goes away!", ++i, lastResult);
                lastResult = diag.ShowDialogModal(this);
                if (Dispatcher.HasShutdownStarted || Dispatcher.HasShutdownStarted) { break; }
            }
        }

        private void btnMsgWindow_Click(object sender, RoutedEventArgs e)
        {
            new MsgWindow { Owner = this }.Show();
        }

        private void btnMsgBox_Click(object sender, RoutedEventArgs e)
        {
            //ModernMessageBox.Show(this, "howdy");
            ModernMessageBox.Show(this,
                "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Cras faucibus venenatis felis a luctus. Cras cursus est sed interdum consectetur. Fusce vestibulum cursus interdum. Praesent ultricies egestas dolor at elementum. Quisque et pellentesque magna, ac mattis purus. Ut pretium laoreet ullamcorper. Morbi venenatis accumsan varius. Interdum et malesuada fames ac ante ipsum primis in faucibus. Vivamus sit amet laoreet leo. Vestibulum sodales tempus libero vitae tincidunt. Nulla facilisi. Donec posuere sapien ut interdum condimentum. Vivamus nec velit suscipit, dignissim odio a, ullamcorper arcu. Proin ac tellus enim. Quisque in cursus dolor. Curabitur adipiscing vitae sem in ornare.\n\n" +
                "Duis in lacus volutpat, laoreet felis eget, tristique mauris. Maecenas dictum porta purus, id fringilla diam suscipit eu. Sed vitae vulputate erat. Praesent sit amet volutpat urna. Aenean id eros tincidunt, tempor nisl ut, malesuada augue. Nullam ullamcorper, sem sed consequat placerat, velit lacus porttitor velit, et suscipit ipsum nisi id lorem. Vivamus eleifend congue erat, ut rhoncus magna lacinia et.\n\n" +
                "Maecenas in sapien vitae ligula interdum vestibulum ac ut odio. Praesent id posuere ligula. In a neque magna. Cras vestibulum fringilla urna, nec aliquam ante. Proin consectetur a enim eget varius. Aliquam vitae nulla mattis, imperdiet sapien eu, hendrerit nulla. In vel lorem mauris. Vestibulum rutrum, lorem suscipit sagittis euismod, justo nulla pharetra augue, cursus semper ante ante mollis ipsum. Phasellus volutpat augue eget consequat pretium. Vivamus sed ante vel purus hendrerit sollicitudin. Aliquam dignissim leo eu enim interdum auctor. Nam augue tortor, scelerisque facilisis turpis at, venenatis imperdiet dui. Nulla facilisi.", "Caption", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning, MessageBoxResult.No);
            ModernMessageBox.Show(this, "Test Message", "Caption", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes);
            ModernMessageBox.Show(this, "Test Message", "Caption", MessageBoxButton.OKCancel, MessageBoxImage.Information, MessageBoxResult.Cancel);
            ModernMessageBox.Show(this, "Test Message", "Caption", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
        }

        private void btnLang_Click(object sender, RoutedEventArgs e)
        {
            btnLang.ContextMenu.Placement = PlacementMode.Top;
            btnLang.ContextMenu.PlacementTarget = btnLang;
            btnLang.ContextMenu.IsOpen = true;
        }

        private void btnSysDiag_Click(object sender, RoutedEventArgs e)
        {
            btnSysDiag.ContextMenu.Placement = PlacementMode.Top;
            btnSysDiag.ContextMenu.PlacementTarget = btnSysDiag;
            btnSysDiag.ContextMenu.IsOpen = true;
        }
    }
}
