using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        bool dark = true;

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (dark)
            {
                ModernTheme.ApplyTheme(ModernWPF.ModernTheme.ThemeType.Light, ModernTheme.GetPredefinedAccent(ModernTheme.RED));
                dark = false;
            }
            else
            {
                ModernTheme.ApplyTheme(ModernWPF.ModernTheme.ThemeType.Dark, ModernTheme.GetPredefinedAccent(ModernTheme.GREEN));
                dark = true;
            }
        }
    }
}
