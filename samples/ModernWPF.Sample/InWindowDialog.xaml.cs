using ModernWPF.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
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
    /// Interaction logic for InWindowDialog.xaml
    /// </summary>
    public partial class InWindowDialog : DialogControl
    {
        public InWindowDialog()
        {
            InitializeComponent();
        }

        public string Message { get { return infolabel.Text; } set { infolabel.Text = value; } }

        protected override void OnFocus()
        {
            mybox.Focus();
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void btnTop_Click(object sender, RoutedEventArgs e)
        {
            VerticalAlignment = System.Windows.VerticalAlignment.Top;
        }

        private void btnCenter_Click(object sender, RoutedEventArgs e)
        {
            VerticalAlignment = System.Windows.VerticalAlignment.Center;
        }

        private void btnBottom_Click(object sender, RoutedEventArgs e)
        {
            VerticalAlignment = System.Windows.VerticalAlignment.Bottom;
        }

        private void btnStretch_Click(object sender, RoutedEventArgs e)
        {
            VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
        }

        private void mybox_KeyDown(object sender, KeyEventArgs e)
        {
            //if (e.Key == Key.Escape) { e.Handled = true; }
        }
    }
}
