using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;

namespace ModernWPF.Internal
{
    static class EventUtil
    {

        public static void AddHandler(DependencyObject element, RoutedEvent routedEvent, Delegate handler)
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
                        throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Invalid element {0}.", element.GetType()));
                }
            }
        }
        public static void RemoveHandler(DependencyObject element, RoutedEvent routedEvent, Delegate handler)
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
                        throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Invalid element {0}.", element.GetType()));
                }
            }
        }
    }
}
