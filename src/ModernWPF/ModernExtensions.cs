using System.Windows;
using System.Windows.Input;
using System;
using System.Windows.Controls;
using System.Windows.Media;
using System.Security.Permissions;
using System.Windows.Threading;

namespace ModernWPF
{
    /// <summary>
    /// Extension methods for using this lib.
    /// </summary>
    public static class ModernExtensions
    {
        ///// <summary>
        ///// Try to the get <see cref="ScrollViewer" /> from an <see cref="ItemsControl" />.
        ///// </summary>
        ///// <param name="control">The control.</param>
        ///// <returns></returns>
        //[Obsolete("Use FindInVisualTree instead.")]
        //public static ScrollViewer TryGetScrollViewer(this ItemsControl control)
        //{
        //    if (control != null && VisualTreeHelper.GetChildrenCount(control) > 0)
        //    {
        //        Decorator border = VisualTreeHelper.GetChild(control, 0) as Decorator;
        //        if (border != null)
        //        {
        //            return border.Child as ScrollViewer;
        //        }
        //    }
        //    return null;
        //}

        /// <summary>
        /// Determines whether the specified element is fully in the viewport of the first scrollviewr parent
        /// (i.e. not clipped).
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns></returns>
        public static bool IsFullyInScrollViewer(this UIElement element)
        {
            // modified from http://blogs.msdn.com/b/llobo/archive/2007/01/18/elements-visibility-inside-scrollviewer.aspx

            if (element != null)
            {
                var scroll = FindParentInVisualTree<ScrollViewer>(element);
                if (scroll != null)
                {
                    // position of your visual inside the scrollviewer    
                    GeneralTransform childTransform = element.TransformToAncestor(scroll);
                    Rect testRect = childTransform.TransformBounds(new Rect(new Point(), element.RenderSize));

                    //Check if the elements Rect intersects with that of the scrollviewer's
                    Rect viewPortRect = new Rect(new Point(), new Size(scroll.ViewportWidth, scroll.ViewportHeight));
                    Rect result = Rect.Intersect(viewPortRect, testRect);

                    // if clipped the result will be diff from element rect.
                    return testRect == result;
                }
            }
            return false;
        }

        /// <summary>
        /// Finds the first parent of the specified type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="control">The control.</param>
        /// <returns></returns>
        public static T FindParentInVisualTree<T>(this DependencyObject control) where T : DependencyObject
        {
            while (control != null && !(control is T))
            {
                control = VisualTreeHelper.GetParent(control);
            }
            return control as T;
        }


        /// <summary>
        /// Finds the first specified object type in visual tree.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="control">The control.</param>
        /// <param name="reverse">if set to <c>true</c> then find in reverse order (last child first).</param>
        /// <returns></returns>
        public static T FindChildInVisualTree<T>(this DependencyObject control, bool reverse = false) where T : DependencyObject
        {
            if (control != null)
            {
                var count = VisualTreeHelper.GetChildrenCount(control);

                if (reverse)
                {
                    for (int i = count - 1; i >= 0; i--)
                    {
                        var c = VisualTreeHelper.GetChild(control, i);
                        var casted = c as T;
                        if (casted != null)
                        {
                            return casted;
                        }
                        else if (c != null)
                        {
                            var subHit = FindChildInVisualTree<T>(c, reverse);
                            if (subHit != null) { return subHit; }
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < count; i++)
                    {
                        var c = VisualTreeHelper.GetChild(control, i);
                        var casted = c as T;
                        if (casted != null)
                        {
                            return casted;
                        }
                        else if (c != null)
                        {
                            var subHit = FindChildInVisualTree<T>(c, reverse);
                            if (subHit != null) { return subHit; }
                        }
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
                    var casted = c as T;
                    if (casted != null)
                    {
                        var result = callback(casted);
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



        internal static bool CanVScrollDown(this ScrollViewer scroller)
        {
            return scroller.ScrollableHeight > 0 && scroller.VerticalOffset < scroller.ScrollableHeight;
        }
        internal static bool CanHScrollRight(this ScrollViewer scroller)
        {
            return scroller.ScrollableWidth > 0 && scroller.HorizontalOffset < scroller.ScrollableWidth;
        }

        internal static bool CanVScrollUp(this ScrollViewer scroller)
        {
            return scroller.ScrollableHeight > 0 && scroller.VerticalOffset > 0;
        }
        internal static bool CanHScrollLeft(this ScrollViewer scroller)
        {
            return scroller.ScrollableWidth > 0 && scroller.HorizontalOffset > 0;
        }





        // from msdn http://msdn.microsoft.com/library/system.windows.threading.dispatcher.pushframe.aspx

        /// <summary>
        /// Simulate the famous DoEvents() method from winform days.
        /// </summary>
        /// <param name="application">The application.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters"), SecurityPermissionAttribute(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        public static void DoEvents(this Application application)
        {
            if (application == null) { throw new ArgumentNullException("application"); }
            application.Dispatcher.DoEvents();
        }

        /// <summary>
        /// Simulate the famous DoEvents() method from winform days.
        /// </summary>
        /// <param name="dispatcher">The dispatcher.</param>
        [SecurityPermissionAttribute(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        public static void DoEvents(this Dispatcher dispatcher)
        {
            if (dispatcher == null) { throw new ArgumentNullException("dispatcher"); }
            DispatcherFrame frame = new DispatcherFrame();
            dispatcher.BeginInvoke(DispatcherPriority.Background, new DispatcherOperationCallback(ExitFrame), frame);
            Dispatcher.PushFrame(frame);
        }

        static object ExitFrame(object f)
        {
            ((DispatcherFrame)f).Continue = false;
            return null;
        }
    }
}
