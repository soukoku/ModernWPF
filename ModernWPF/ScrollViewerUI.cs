using ModernWPF.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ModernWPF
{
    /// <summary>
    /// Contains various attached properties for <see cref="ScrollViewer"/> using the modern theme.
    /// </summary>
    public static class ScrollViewerUI
    {

        #region HScroll attached dp

        #region fake h wheel

        /// <summary>
        /// Attached propert to do horizontal scroll by normal (vertical) mouse wheel if possible.
        /// </summary>
        public static readonly DependencyProperty SimulateHWheelProperty =
            DependencyProperty.RegisterAttached
            (
                "SimulateHWheel",
                typeof(bool),
                typeof(ScrollViewerUI),
                new PropertyMetadata(false, OnSimulateHWheelPropertyChanged)
            );

        /// <summary>
        /// Gets the SimulateHWheel property for this object.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        public static bool GetSimulateHWheel(DependencyObject obj)
        {
            if (obj == null) { throw new ArgumentNullException("obj"); }
            return (bool)obj.GetValue(SimulateHWheelProperty);
        }
        /// <summary>
        /// Sets the SimulateHWheel property for this object.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="value">if set to <c>true</c> then h-scroll logic will be used if possible.</param>
        public static void SetSimulateHWheel(DependencyObject obj, bool value)
        {
            if (obj == null) { throw new ArgumentNullException("obj"); }
            obj.SetValue(SimulateHWheelProperty, value);
        }

        private static void OnSimulateHWheelPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (DesignerProperties.GetIsInDesignMode(d)) { return; }

            var scroller = d as ScrollViewer;
            if (scroller == null)// dpo is ItemsControl)
            {
                //scroller = ((ItemsControl)dpo).TryGetScrollerViewer();
                scroller = d.FindInVisualTree<ScrollViewer>();
            }

            if (scroller != null)
            {
                scroller.PreviewMouseWheel -= HandleVWheelEvent;
                if ((bool)e.NewValue)
                {
                    scroller.PreviewMouseWheel += HandleVWheelEvent;
                }
            }
        }

        // this simulates h scroll if at end/top of regular v scroll
        static void HandleVWheelEvent(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer scroller = sender as ScrollViewer;

            // only allow h-scroll if not doing v-scroll

            if (scroller != null)
            {
                if (e.Delta < 0)
                {
                    if (!scroller.CanVScrollDown() && scroller.CanHScrollRight())
                    {
                        //scroller.LineRight();
                        scroller.ScrollToHorizontalOffset(scroller.HorizontalOffset + 48);
                        e.Handled = true;
                    }
                }
                else
                {
                    if (!scroller.CanVScrollUp() && scroller.CanHScrollLeft())
                    {
                        //scroller.LineLeft();
                        scroller.ScrollToHorizontalOffset(scroller.HorizontalOffset - 48);
                        e.Handled = true;
                    }
                }
            }
        }

        #endregion

        #region real h wheel

        /// <summary>
        /// Attached propert to handle horizontal wheel on <see cref="ScrollViewer"/>.
        /// </summary>
        public static readonly DependencyProperty HandleHWheelProperty =
            DependencyProperty.RegisterAttached
            (
                "HandleHWheel",
                typeof(bool),
                typeof(ScrollViewerUI),
                new PropertyMetadata(false, OnHandleHWheelPropertyChanged)
            );
        /// <summary>
        /// Gets the HandleHWheel property for this object.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        public static bool GetHandleHWheel(DependencyObject obj)
        {
            if (obj == null) { throw new ArgumentNullException("obj"); }
            return (bool)obj.GetValue(HandleHWheelProperty);
        }
        /// <summary>
        /// Sets the HandleHWheel property for this object.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="value">if set to <c>true</c> then h-scroll logic will be used if possible.</param>
        public static void SetHandleHWheel(DependencyObject obj, bool value)
        {
            if (obj == null) { throw new ArgumentNullException("obj"); }
            obj.SetValue(HandleHWheelProperty, value);
        }

        private static void OnHandleHWheelPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (DesignerProperties.GetIsInDesignMode(d)) { return; }

            var scroller = d as ScrollViewer;
            if (scroller == null)// dpo is ItemsControl)
            {
                //scroller = ((ItemsControl)dpo).TryGetScrollerViewer();
                scroller = d.FindInVisualTree<ScrollViewer>();
            }

            // animated scroll viewer handles this already so do nothing
            if (scroller != null && !(scroller is AnimatedScrollViewer))
            {
                scroller.RemoveHandler(MouseEvents.MouseHWheelEvent, new MouseWheelEventHandler(HandleRealHWheelEvent));
                if ((bool)e.NewValue)
                {
                    scroller.AddHandler(MouseEvents.MouseHWheelEvent, new MouseWheelEventHandler(HandleRealHWheelEvent));
                }
            }
        }

        static void HandleRealHWheelEvent(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer scroller = sender as ScrollViewer;
            if (scroller != null)
            {
                if (e.Delta > 0)
                {
                    if (scroller.CanHScrollRight())
                    {
                        //scroller.LineRight();
                        scroller.ScrollToHorizontalOffset(scroller.HorizontalOffset + 48);
                        e.Handled = true;
                    }
                }
                else
                {
                    if (scroller.CanHScrollLeft())
                    {
                        //scroller.LineLeft();
                        scroller.ScrollToHorizontalOffset(scroller.HorizontalOffset - 48);
                        e.Handled = true;
                    }
                }
            }
        }
        #endregion

        #endregion

        #region over content dp

        /// <summary>
        /// Gets whether the scrollbar will cover the content.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns></returns>
        public static bool GetOverContent(DependencyObject obj)
        {
            if (obj == null) { throw new ArgumentNullException("obj"); }
            return (bool)obj.GetValue(OverContentProperty);
        }

        /// <summary>
        /// Sets whether the scrollbar will cover the content.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="value">The value flag.</param>
        public static void SetOverContent(DependencyObject obj, bool value)
        {
            if (obj == null) { throw new ArgumentNullException("obj"); }
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
