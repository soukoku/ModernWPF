using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ModernWPF
{
    /// <summary>
    /// Contains various attached properties for <see cref="TextBox"/> and <see cref="PasswordBox"/> using the modern theme.
    /// </summary>
    public class TextBoxUI : DependencyObject
    {

        /// <summary>
        /// Gets the show clear button flag.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns></returns>
        public static bool GetShowClearButton(DependencyObject obj)
        {
            return (bool)obj.GetValue(ShowClearButtonProperty);
        }

        /// <summary>
        /// Sets the show clear button flag.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="value">the flag value.</param>
        public static void SetShowClearButton(DependencyObject obj, bool value)
        {
            obj.SetValue(ShowClearButtonProperty, value);
        }

        /// <summary>
        /// The DP flag on whether to show clear text button in a textbox in modern theme.
        /// </summary>
        public static readonly DependencyProperty ShowClearButtonProperty =
            DependencyProperty.RegisterAttached("ShowClearButton", typeof(bool), typeof(TextBoxUI), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.Inherits));




        /// <summary>
        /// Gets the text box leading element.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns></returns>
        public static UIElement GetLeadingElement(DependencyObject obj)
        {
            return (UIElement)obj.GetValue(LeadingElementProperty);
        }

        /// <summary>
        /// Sets the text box leading element.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="value">The value.</param>
        public static void SetLeadingElement(DependencyObject obj, UIElement value)
        {
            obj.SetValue(LeadingElementProperty, value);
        }

        /// <summary>
        /// The DP on a text box in modern theme that contains a leading element (e.g. a search icon).
        /// </summary>
        public static readonly DependencyProperty LeadingElementProperty =
            DependencyProperty.RegisterAttached("LeadingElement", typeof(UIElement), typeof(TextBoxUI), new PropertyMetadata(null));


        /// <summary>
        /// Gets the text box trailing element.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns></returns>
        public static UIElement GetTrailingElement(DependencyObject obj)
        {
            return (UIElement)obj.GetValue(TrailingElementProperty);
        }

        /// <summary>
        /// Sets the text box trailing element.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="value">The value.</param>
        public static void SetTrailingElement(DependencyObject obj, UIElement value)
        {
            obj.SetValue(TrailingElementProperty, value);
        }

        /// <summary>
        /// The DP on a text box in modern theme that contains a trailing element (e.g. a search icon).
        /// </summary>
        public static readonly DependencyProperty TrailingElementProperty =
            DependencyProperty.RegisterAttached("TrailingElement", typeof(UIElement), typeof(TextBoxUI), new PropertyMetadata(null));

        // password part modified from http://brentstewart.net/blog/post/2013/02/18/How-to-add-watermark-text-to-a-PasswordBox


        /// <summary>
        /// Gets the text box water mark text.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns></returns>
        public static string GetWatermarkText(DependencyObject obj)
        {
            return (string)obj.GetValue(WatermarkTextProperty);
        }

        /// <summary>
        /// Sets the text box water mark text.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="value">The value.</param>
        public static void SetWatermarkText(DependencyObject obj, string value)
        {
            obj.SetValue(WatermarkTextProperty, value);
        }

        /// <summary>
        /// The DP on a text box in modern theme that shows this watermark when empty.
        /// </summary>
        public static readonly DependencyProperty WatermarkTextProperty =
            DependencyProperty.RegisterAttached("WatermarkText", typeof(string), typeof(TextBoxUI), new PropertyMetadata(null, WatermarkChanged));

        static void WatermarkChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // extra handling for password
            // leak if never got unset?
            var pb = d as PasswordBox;
            if (pb != null)
            {
                if (e.NewValue == null)
                {
                    pb.PasswordChanged -= pb_PasswordChanged;
                }
                else
                {
                    pb.PasswordChanged -= pb_PasswordChanged;
                    pb.PasswordChanged += pb_PasswordChanged;
                }
            }
        }

        static void pb_PasswordChanged(object sender, RoutedEventArgs e)
        {
            var pb = sender as PasswordBox;
            if (pb != null)
            {
                pb.SetValue(HasPasswordProperty, pb.SecurePassword.Length > 0);
            }
        }


        #region HasPassword
        /// <summary>
        /// Gets a value indicating whether a password box has password for watermark purposes.
        /// </summary>
        /// <value>
        ///   <c>true</c> if it has password; otherwise, <c>false</c>.
        /// </value>
        public bool HasPassword
        {
            get { return (bool)GetValue(HasPasswordProperty); }
            //set { SetValue(HasPasswordProperty, value); }
        }

        private static readonly DependencyProperty HasPasswordProperty =
            DependencyProperty.RegisterAttached("HasPassword", typeof(bool), typeof(TextBoxUI), new PropertyMetadata(false));

        #endregion
    }
}
