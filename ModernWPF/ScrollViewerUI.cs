using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ModernWPF
{
    /// <summary>
    /// Contains various attached properties for <see cref="ScrollViewer"/> using the modern theme..
    /// </summary>
    public class ScrollViewerUI
    {

        #region HScroll attached dp

        /// <summary>
        /// Attached propert to allow horizontal scroll by mouse wheel if possible.
        /// </summary>
        public static readonly DependencyProperty HScrollOnWheelProperty =
            DependencyProperty.RegisterAttached
            (
                "HScrollOnWheel",
                typeof(bool),
                typeof(ScrollViewerUI),
                new PropertyMetadata(false, OnHScrollOnWheelPropertyChanged)
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
            var scroller = dpo as ScrollViewer;
            if (dpo is ItemsControl)
            {
                scroller = ((ItemsControl)dpo).TryGetScrollerViewer();
            }

            if (scroller != null)
            {
                if ((bool)args.NewValue)
                {
                    scroller.PreviewMouseWheel -= scroller_PreviewMouseWheel;
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
                    //scroller.LineRight();
                    scroller.ScrollToHorizontalOffset(scroller.HorizontalOffset + 48);
                }
                else
                {
                    //scroller.LineLeft();
                    scroller.ScrollToHorizontalOffset(scroller.HorizontalOffset - 48);
                }
                e.Handled = true;
            }
        }

        #endregion

        #region over content dp

        /// <summary>
        /// Gets whether the scrollbar will cover the content.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns></returns>
        public static bool GetOverContent(DependencyObject obj)
        {
            return (bool)obj.GetValue(OverContentProperty);
        }

        /// <summary>
        /// Sets whether the scrollbar will cover the content.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="value">The value flag.</param>
        public static void SetOverContent(DependencyObject obj, bool value)
        {
            obj.SetValue(OverContentProperty, value);
        }

        /// <summary>
        /// DP on whether the scrollbar will cover the content.
        /// </summary>
        public static readonly DependencyProperty OverContentProperty =
            DependencyProperty.RegisterAttached("OverContent", typeof(bool), typeof(ScrollViewerUI), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.Inherits));

        #endregion

    }
}
