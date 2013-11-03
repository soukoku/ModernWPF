using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ModernWPF.Behaviors
{
    /// <summary>
    /// Contains behaviors for the metro lib.
    /// </summary>
    public class ModernBehavior
    {
        #region HScroll

        /// <summary>
        /// Attached propert to allow horizontal scroll by mouse wheel if possible.
        /// </summary>
        public static readonly DependencyProperty HScrollOnWheelProperty =
            DependencyProperty.RegisterAttached
            (
                "HScrollOnWheel",
                typeof(bool),
                typeof(ModernBehavior),
                new UIPropertyMetadata(false, OnHScrollOnWheelPropertyChanged)
            );
        /// <summary>
        /// Gets the HScrollOnWheel property for this object.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        public static bool GetHScrollOnWheel(DependencyObject obj)
        {
            return (bool)obj.GetValue(HScrollOnWheelProperty);
        }
        /// <summary>
        /// Sets the HScrollOnWheel property for this object.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="value">if set to <c>true</c> then h-scroll logic will be used if possible.</param>
        public static void SetHScrollOnWheel(DependencyObject obj, bool value)
        {
            obj.SetValue(HScrollOnWheelProperty, value);
        }

        private static void OnHScrollOnWheelPropertyChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            ScrollViewer scroller = dpo as ScrollViewer;
            if (dpo is ItemsControl)
            {
                scroller = ((ItemsControl)dpo).TryGetScrollerViewer();
            }

            if (scroller != null)
            {
                if ((bool)args.NewValue)
                {
                    scroller.PreviewMouseWheel += scroller_PreviewMouseWheel;
                }
                else
                {
                    scroller.PreviewMouseWheel -= scroller_PreviewMouseWheel;
                }
            }
        }

        static void scroller_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer scroller = sender as ScrollViewer;
            if (scroller != null && scroller.ComputedVerticalScrollBarVisibility != Visibility.Visible &&
                scroller.ComputedHorizontalScrollBarVisibility == Visibility.Visible)
            {
                if (e.Delta < 0)
                {
                    scroller.ScrollToHorizontalOffset(scroller.HorizontalOffset + 48);
                }
                else
                {
                    scroller.ScrollToHorizontalOffset(scroller.HorizontalOffset - 48);
                }
                e.Handled = true;
            }
        }

        #endregion
    }
}
