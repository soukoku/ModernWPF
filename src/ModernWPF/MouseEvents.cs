using System;
using System.Collections.Generic;
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
            AddHandler(element, MouseEvents.PreviewMouseHWheelEvent, (Delegate)handler);
        }

        /// <summary>
        /// Removes a handler to the PreviewMouseHWheel event.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="handler">The handler.</param>
        public static void RemovePreviewMouseHWheelHandler(DependencyObject element, MouseWheelEventHandler handler)
        {
            RemoveHandler(element, MouseEvents.PreviewMouseHWheelEvent, (Delegate)handler);
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
            AddHandler(element, MouseEvents.MouseHWheelEvent, (Delegate)handler);
        }

        /// <summary>
        /// Removes a handler to the MouseHWheel event.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="handler">The handler.</param>
        public static void RemoveMouseHWheelHandler(DependencyObject element, MouseWheelEventHandler handler)
        {
            RemoveHandler(element, MouseEvents.MouseHWheelEvent, (Delegate)handler);
        }

        static void AddHandler(DependencyObject element, RoutedEvent routedEvent, Delegate handler)
        {
            if (element == null) { throw new ArgumentNullException("element"); }

            var uie = element as UIElement;
            if (uie != null)
            {
                uie.AddHandler(routedEvent, handler);
            }
            else
            {
                var ce = element as ContentElement;
                if (ce != null)
                {
                    ce.AddHandler(routedEvent, handler);
                }
                else
                {
                    var u3d = element as UIElement3D;
                    if (u3d != null)
                        u3d.AddHandler(routedEvent, handler);
                    else
                        throw new ArgumentException(string.Format("Invalid element {0}.", element.GetType()));
                }
            }
        }
        static void RemoveHandler(DependencyObject element, RoutedEvent routedEvent, Delegate handler)
        {
            if (element == null) { throw new ArgumentNullException("element"); }

            var uie = element as UIElement;
            if (uie != null)
            {
                uie.RemoveHandler(routedEvent, handler);
            }
            else
            {
                var ce = element as ContentElement;
                if (ce != null)
                {
                    ce.RemoveHandler(routedEvent, handler);
                }
                else
                {
                    var u3d = element as UIElement3D;
                    if (u3d != null)
                        u3d.RemoveHandler(routedEvent, handler);
                    else
                        throw new ArgumentException(string.Format("Invalid element {0}.", element.GetType()));
                }
            }
        }
    }
}
