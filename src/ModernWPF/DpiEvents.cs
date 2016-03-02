using Microsoft.Win32;
using ModernWPF.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace ModernWPF
{
    public class DpiEvents : DependencyObject
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


        internal static Dictionary<int, int> WindowDpis { get; } = new Dictionary<int, int>();

        private DpiEvents()
        { }
        private static DpiEvents __instance = new DpiEvents();

        public static DpiEvents Instance
        {
            get { return __instance; }
        }


        public object BindingHack
        {
            get { return GetValue(BindingHackProperty); }
            set { SetValue(BindingHackProperty, value); }
        }


        public static readonly DependencyProperty BindingHackProperty =
            DependencyProperty.Register("BindingHack", typeof(object), typeof(DpiEvents), new PropertyMetadata(null));



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
        public DpiChangeEventArgs(Window window, int newDpi)
        {
            Window = window;
            NewDpi = newDpi;
        }

        /// <summary>
        /// Gets the root window this DPI change affects.
        /// </summary>
        /// <value>
        /// The window.
        /// </value>
        public Window Window { get; internal set; }

        /// <summary>
        /// Gets the new dpi value.
        /// </summary>
        /// <value>
        /// The new dpi.
        /// </value>
        public int NewDpi { get; private set; }

    }
}
