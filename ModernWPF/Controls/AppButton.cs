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
    /// A button with circle around the content and optional text.
    /// </summary>
    public class AppButton : ToggleButton
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline")]
        static AppButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(AppButton), new FrameworkPropertyMetadata(typeof(AppButton)));
        }

        /// <summary>
        /// Gets or sets the orientation for the icon and text.
        /// </summary>
        /// <value>
        /// The orientation.
        /// </value>
        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        /// <summary>
        /// Dependency property for <see cref="Orientation"/>.
        /// </summary>
        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register("Orientation", typeof(Orientation), typeof(AppButton), new PropertyMetadata(Orientation.Vertical));



        /// <summary>
        /// Gets or sets the button size.
        /// </summary>
        /// <value>
        /// The size.
        /// </value>
        public AppButtonSize ButtonSize
        {
            get { return (AppButtonSize)GetValue(ButtonSizeProperty); }
            set { SetValue(ButtonSizeProperty, value); }
        }

        /// <summary>
        /// Dependency property for <see cref="ButtonSize"/>.
        /// </summary>
        public static readonly DependencyProperty ButtonSizeProperty =
            DependencyProperty.Register("ButtonSize", typeof(AppButtonSize), typeof(AppButton), new PropertyMetadata(AppButtonSize.Large));




        /// <summary>
        /// Gets or sets the size of the text.
        /// </summary>
        /// <value>
        /// The size of the text.
        /// </value>
        public AppButtonSize TextSize
        {
            get { return (AppButtonSize)GetValue(TextSizeProperty); }
            set { SetValue(TextSizeProperty, value); }
        }

        /// <summary>
        /// Dependency property for <see cref="TextSize"/>.
        /// </summary>
        public static readonly DependencyProperty TextSizeProperty =
            DependencyProperty.Register("TextSize", typeof(AppButtonSize), typeof(AppButton), new PropertyMetadata(AppButtonSize.Small));

        


        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>
        /// The text.
        /// </value>
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }


        /// <summary>
        /// Dependency property for <see cref="Text"/>.
        /// </summary>
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(AppButton), new PropertyMetadata(null));




        /// <summary>
        /// Gets or sets a value indicating whether the button will collapse if not enabled.
        /// Default is true.
        /// </summary>
        /// <value>
        ///   <c>true</c> to collapse on disable; otherwise, <c>false</c>.
        /// </value>
        public bool CollapseOnDisable
        {
            get { return (bool)GetValue(CollapseOnDisableProperty); }
            set { SetValue(CollapseOnDisableProperty, value); }
        }


        /// <summary>
        /// Dependency property for <see cref="CollapseOnDisable"/>.
        /// </summary>
        public static readonly DependencyProperty CollapseOnDisableProperty =
            DependencyProperty.Register("CollapseOnDisable", typeof(bool), typeof(AppButton), new PropertyMetadata(true));

        protected override void OnContextMenuOpening(ContextMenuEventArgs e)
        {
            e.Handled = true;
            base.OnContextMenuOpening(e);
        }
    }

    /// <summary>
    /// Indicates the text or icon size for <see cref="AppButton"/>.
    /// </summary>
    public enum AppButtonSize
    {
        /// <summary>
        /// Use the large size.
        /// </summary>
        Large,
        /// <summary>
        /// Use the small size.
        /// </summary>
        Small
    }
}
