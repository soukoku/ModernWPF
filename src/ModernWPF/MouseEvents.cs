using ModernWPF.Internal;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace ModernWPF
{
    /// <summary>
    /// Contains extra mouse events when using the modern <see cref="Chrome"/> on a <see cref="Window"/>.
    /// </summary>
    public static class MouseEvents
    {
        /// <summary>
        /// Identifies the PreviewMouseHWheel event.
        /// </summary>
        public static readonly RoutedEvent PreviewMouseHWheelEvent =
            EventManager.RegisterRoutedEvent("PreviewMouseHWheel", RoutingStrategy.Tunnel, typeof(MouseWheelEventHandler), typeof(MouseEvents));


        /// <summary>
        /// Adds a handler to the PreviewMouseHWheel event.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="handler">The handler.</param>
        public static void AddPreviewMouseHWheelHandler(DependencyObject element, MouseWheelEventHandler handler)
        {
            EventUtil.AddHandler(element, MouseEvents.PreviewMouseHWheelEvent, (Delegate)handler);
        }

        /// <summary>
        /// Removes a handler to the PreviewMouseHWheel event.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="handler">The handler.</param>
        public static void RemovePreviewMouseHWheelHandler(DependencyObject element, MouseWheelEventHandler handler)
        {
            EventUtil.RemoveHandler(element, MouseEvents.PreviewMouseHWheelEvent, (Delegate)handler);
        }


        /// <summary>
        /// Identifies the MouseHWheel event.
        /// </summary>
        public static readonly RoutedEvent MouseHWheelEvent =
            EventManager.RegisterRoutedEvent("MouseHWheel", RoutingStrategy.Bubble, typeof(MouseWheelEventHandler), typeof(MouseEvents));

        /// <summary>
        /// Adds a handler to the MouseHWheel event.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="handler">The handler.</param>
        public static void AddMouseHWheelHandler(DependencyObject element, MouseWheelEventHandler handler)
        {
            EventUtil.AddHandler(element, MouseEvents.MouseHWheelEvent, (Delegate)handler);
        }

        /// <summary>
        /// Removes a handler to the MouseHWheel event.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="handler">The handler.</param>
        public static void RemoveMouseHWheelHandler(DependencyObject element, MouseWheelEventHandler handler)
        {
            EventUtil.RemoveHandler(element, MouseEvents.MouseHWheelEvent, (Delegate)handler);
        }
    }
}
