using Microsoft.Win32;
using ModernWPF.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ModernWPF
{
    public class DpiEvents
    {
        /// <summary>
        /// Identifies the DpiChange event. This can only be listened to by a <see cref="Window"/>.
        /// </summary>
        public static readonly RoutedEvent DpiChangeEvent =
            EventManager.RegisterRoutedEvent("DpiChange", RoutingStrategy.Direct, typeof(EventHandler<DpiChangeEventArgs>), typeof(DpiEvents));

        /// <summary>
        /// Adds a handler to the DpiChange event.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="handler">The handler.</param>
        public static void AddDpiChangeHandler(DependencyObject element, EventHandler<DpiChangeEventArgs> handler)
        {
            EventUtil.AddHandler(element, DpiEvents.DpiChangeEvent, (Delegate)handler);
        }

        /// <summary>
        /// Removes a handler to the DpiChange event.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="handler">The handler.</param>
        public static void RemoveDpiChangeHandler(DependencyObject element, EventHandler<DpiChangeEventArgs> handler)
        {
            EventUtil.RemoveHandler(element, DpiEvents.DpiChangeEvent, (Delegate)handler);
        }


        #region DPI attached prop

        /// <summary>
        /// Attached property on a window to store its current DPI scale value.
        /// </summary>
        private static readonly DependencyProperty WindowDpiScaleProperty =
            DependencyProperty.RegisterAttached("WindowDpiScale", typeof(double), typeof(DpiEvents),
            new FrameworkPropertyMetadata(1d, FrameworkPropertyMetadataOptions.Inherits));

        /// <summary>
        /// Gets the dpi scale value for the object contained in a window using <see cref="Chrome"/>.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns></returns>
        public static double GetWindowDpiScale(DependencyObject obj)
        {
            return (double)obj.GetValue(WindowDpiScaleProperty);
        }

        internal static void SetWindowDpiScale(DependencyObject obj, double dpiScale)
        {
            obj.SetValue(WindowDpiScaleProperty, dpiScale);
        }

        /// <summary>
        /// Attached property on a window to store its current DPI value.
        /// </summary>
        private static readonly DependencyProperty WindowDpiProperty =
            DependencyProperty.RegisterAttached("WindowDpi", typeof(int), typeof(DpiEvents),
            new FrameworkPropertyMetadata(96, FrameworkPropertyMetadataOptions.Inherits));

        /// <summary>
        /// Gets the dpi value for the object contained in a window using <see cref="Chrome"/>.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns></returns>
        public static int GetWindowDpi(DependencyObject obj)
        {
            return (int)obj.GetValue(WindowDpiProperty);
        }

        internal static void SetWindowDpi(DependencyObject obj, int dpi)
        {
            obj.SetValue(WindowDpiProperty, dpi);
        }




        internal static bool GetIsDpiTransform(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsDpiTransformProperty);
        }

        internal static void SetIsDpiTransform(DependencyObject obj, bool value)
        {
            obj.SetValue(IsDpiTransformProperty, value);
        }

        // Using a DependencyProperty as the backing store for IsDpiTransform.  This enables animation, styling, binding, etc...
        static readonly DependencyProperty IsDpiTransformProperty =
            DependencyProperty.RegisterAttached("IsDpiTransform", typeof(bool), typeof(DpiEvents), new PropertyMetadata(false));



        #endregion

        /// <summary>
        /// Scales the element based on some factor.
        /// </summary>
        /// <param name="child">The child.</param>
        /// <param name="scaleFactor">The scale factor.</param>
        /// <param name="compensateRender">if set to <c>true</c> to compensate RTL bug with render transform.</param>
        public static void ScaleElement(FrameworkElement child, double scaleFactor, bool compensateRender = false)
        {
            var flow = child.FlowDirection;
            var origLayout = UnwrapDpiTransform((Transform)child.GetValue(FrameworkElement.LayoutTransformProperty));
            var origRender = UnwrapDpiTransform((Transform)child.GetValue(UIElement.RenderTransformProperty));

            if (scaleFactor != 1.0)
            {
                child.SetValue(FrameworkElement.LayoutTransformProperty, WrapDpiTransform(origLayout, scaleFactor));
                if (compensateRender)
                {
                    // weird wpf bug when using RTL so compensate again in render xform
                    if (flow == FlowDirection.RightToLeft)
                    {
                        child.SetValue(UIElement.RenderTransformProperty, WrapDpiTransform(origRender, scaleFactor));
                    }
                    else
                    {
                        child.SetValue(UIElement.RenderTransformProperty, origRender);
                    }
                }
            }
            else
            {
                child.SetValue(FrameworkElement.LayoutTransformProperty, origLayout);
                if (compensateRender)
                {
                    child.SetValue(UIElement.RenderTransformProperty, origRender);
                }
            }
        }

        static Transform WrapDpiTransform(Transform origTransform, double dpiScaleFactor)
        {
            var group = new TransformGroup();
            if (origTransform != null)
            {
                group.Children.Add(origTransform);
            }
            group.Children.Add(new ScaleTransform(dpiScaleFactor, dpiScaleFactor));
            DpiEvents.SetIsDpiTransform(group, true);
            return group;
        }

        static Transform UnwrapDpiTransform(Transform currentTransform)
        {
            if (currentTransform != null && DpiEvents.GetIsDpiTransform(currentTransform))
            {
                var group = currentTransform as TransformGroup;
                if (group != null && group.Children.Count > 1)
                {
                    return group.Children[0];
                }
                return null;
            }
            return currentTransform;
        }

    }

    /// <summary>
    /// Contains information on DPI changes.
    /// </summary>
    /// <seealso cref="System.Windows.RoutedEventArgs" />
    public class DpiChangeEventArgs : RoutedEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DpiChangeEventArgs" /> class.
        /// </summary>
        /// <param name="window">The window.</param>
        /// <param name="newDpi">The new dpi.</param>
        /// <param name="scale">The scale.</param>
        public DpiChangeEventArgs(Window window, int newDpi, double scale) : base(DpiEvents.DpiChangeEvent, window)
        {
            NewDpi = newDpi;
            Scale = scale;
        }

        /// <summary>
        /// Gets the new dpi value.
        /// </summary>
        /// <value>
        /// The new dpi.
        /// </value>
        public int NewDpi { get; private set; }

        /// <summary>
        /// Gets the scale for the new dpi value.
        /// </summary>
        /// <value>
        /// The scale.
        /// </value>
        public double Scale { get; private set; }
    }
}
