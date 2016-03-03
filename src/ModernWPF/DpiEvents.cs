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

        const int DefaultDpi = 96;

        /// <summary>
        /// Attached property on a window to store its current DPI value.
        /// </summary>
        private static readonly DependencyProperty WindowDpiProperty =
            DependencyProperty.RegisterAttached("WindowDpi", typeof(int), typeof(DpiEvents),
            new FrameworkPropertyMetadata(DefaultDpi, FrameworkPropertyMetadataOptions.Inherits));

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

        #endregion


    }

    /// <summary>
    /// Contains information on DPI changes.
    /// </summary>
    /// <seealso cref="System.Windows.RoutedEventArgs" />
    public class DpiChangeEventArgs : RoutedEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DpiChangeEventArgs"/> class.
        /// </summary>
        /// <param name="window">The window.</param>
        /// <param name="newDpi">The new dpi.</param>
        public DpiChangeEventArgs(Window window, int newDpi) : base(DpiEvents.DpiChangeEvent, window)
        {
            NewDpi = newDpi;
        }

        /// <summary>
        /// Gets the new dpi value.
        /// </summary>
        /// <value>
        /// The new dpi.
        /// </value>
        public int NewDpi { get; private set; }

    }
}
