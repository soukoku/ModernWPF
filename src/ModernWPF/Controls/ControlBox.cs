using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace ModernWPF.Controls
{
    /// <summary>
    /// A UI piece for window control box (min/max/restore buttons).
    /// </summary>
    public class ControlBox : Control
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline")]
        static ControlBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ControlBox), new FrameworkPropertyMetadata(typeof(ControlBox)));
            IsTabStopProperty.OverrideMetadata(typeof(ControlBox), new FrameworkPropertyMetadata(false));
        }



        /// <summary>
        /// Gets or sets the button style.
        /// </summary>
        /// <value>
        /// The button style.
        /// </value>
        public Style ButtonStyle
        {
            get { return (Style)GetValue(ButtonStyleProperty); }
            set { SetValue(ButtonStyleProperty, value); }
        }

        /// <summary>
        /// The button style dependency property.
        /// </summary>
        public static readonly DependencyProperty ButtonStyleProperty =
            DependencyProperty.Register("ButtonStyle", typeof(Style), typeof(ControlBox), new PropertyMetadata(null));

        
    }
}
