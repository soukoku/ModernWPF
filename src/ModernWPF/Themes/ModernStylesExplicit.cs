using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace ModernWPF.Themes
{
    partial class ModernStylesExplicit : ResourceDictionary
    {
        public ModernStylesExplicit()
        {
            InitializeComponent();
        }


        private void Handle_ContextMenuOpened(object sender, RoutedEventArgs e)
        {
            var cm = sender as ContextMenu;
            if (cm != null && cm.PlacementTarget != null)
            {
                var scale = DpiEvents.GetWindowDpiScale(cm.PlacementTarget);
                DpiEvents.ScaleElement(cm, scale);
                Debug.WriteLine("Menu scale = " + scale);
            }
        }
        private void Handle_ToolTipOpened(object sender, RoutedEventArgs e)
        {
            var cm = sender as ToolTip;
            if (cm != null && cm.PlacementTarget != null)
            {
                var scale = DpiEvents.GetWindowDpiScale(cm.PlacementTarget);
                DpiEvents.ScaleElement(cm, scale);
                Debug.WriteLine("ToolTip scale = " + scale);
            }
        }
    }
}
