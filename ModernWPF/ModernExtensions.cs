using System.Windows;
using System.Windows.Input;
using System;
using System.Windows.Controls;
using System.Windows.Media;

namespace ModernWPF
{
    /// <summary>
    /// Extension methods for using this lib.
    /// </summary>
    public static class ModernExtensions
    {
        /// <summary>
        /// Try to the get <see cref="ScrollViewer" /> from an <see cref="ItemsControl" />.
        /// </summary>
        /// <param name="control">The control.</param>
        /// <returns></returns>
        public static ScrollViewer TryGetScrollerViewer(this ItemsControl control)
        {
            if (control != null && VisualTreeHelper.GetChildrenCount(control) > 0)
            {
                Decorator border = VisualTreeHelper.GetChild(control, 0) as Decorator;
                if (border != null)
                {
                    return border.Child as ScrollViewer;
                }
            }
            return null;
        }


        /// <summary>
        /// Finds the first specified object type in visual tree.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="control">The control.</param>
        /// <returns></returns>
        public static T FindInVisualTree<T>(this DependencyObject control) where T : DependencyObject
        {
            if (control != null)
            {
                var count = VisualTreeHelper.GetChildrenCount(control);

                for (int i = 0; i < count; i++)
                {
                    var c = VisualTreeHelper.GetChild(control, i);
                    if (c is T)
                    {
                        return c as T;
                    }
                    else if (c != null)
                    {
                        var subHit = FindInVisualTree<T>(c);
                        if (subHit != null) { return subHit; }
                    }
                }
            }
            return null;
        }

        internal static bool ProcessInVisualTree<T>(this DependencyObject control, Predicate<T> callback) where T : DependencyObject
        {
            if (control != null)
            {
                var count = VisualTreeHelper.GetChildrenCount(control);

                for (int i = 0; i < count; i++)
                {
                    var c = VisualTreeHelper.GetChild(control, i);
                    if (c is T)
                    {
                        var result = callback(c as T);
                        if (result) { return true; }
                    }
                    if (c != null)
                    {
                        var subResult = ProcessInVisualTree<T>(c, callback);
                        if (subResult) { return true; }
                    }
                }
            }
            return false;
        }

    }
}
