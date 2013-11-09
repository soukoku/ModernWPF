using System;
using System.Collections.Generic;
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

            List<string> list = new List<string>();
            for (int i = 1; i <= 1000; i++)
            {
                list.Add(string.Format("should virtual {0}", i)); 
            }
            virtualListBox.ItemsSource = list;
        }


        private void btnTheme_Click(object sender, RoutedEventArgs e)
        {
            if (ModernTheme.CurrentTheme.GetValueOrDefault() == ModernTheme.Theme.Dark)
            {
                ModernTheme.ApplyTheme(ModernTheme.Theme.Light, ModernTheme.CurrentAccent);
            }
            else
            {
                ModernTheme.ApplyTheme(ModernTheme.Theme.Dark, ModernTheme.CurrentAccent);
            }
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            var selected = (sender as RadioButton).DataContext as Accent;
            ModernTheme.ApplyTheme(ModernTheme.CurrentTheme.GetValueOrDefault(), selected);
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
            var diag = new InWindowDialog();
            diag.ShowDialog(this);
        }
    }
}
