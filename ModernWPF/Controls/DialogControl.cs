using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace ModernWPF.Controls
{
    /// <summary>
    /// A user control that can be hosted in a <see cref="DialogControlContainer"/> like a dialog.
    /// </summary>
    public class DialogControl : UserControl
    {
        static DialogControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DialogControl), new FrameworkPropertyMetadata(typeof(DialogControl)));
        }

        #region properties

        internal DialogControlContainer Container { get; set; }

        private bool? _diaglogResult;

        /// <summary>
        /// Gets or sets the dialog result.
        /// </summary>
        /// <value>
        /// The dialog result.
        /// </value>
        [TypeConverter(typeof(DialogResultConverter))]
        public bool? DialogResult
        {
            get { return _diaglogResult; }
            set
            {
                _diaglogResult = value;
                if (Container != null)
                {
                    Container.Close(this);
                }
            }
        }


        /// <summary>
        /// Gets or sets a value indicating whether the dialog closes on escape key.
        /// </summary>
        /// <value>
        /// <c>true</c> to close on escape key; otherwise, <c>false</c>.
        /// </value>
        public bool CloseOnEscapeKey
        {
            get { return (bool)GetValue(CloseOnEscapeKeyProperty); }
            set { SetValue(CloseOnEscapeKeyProperty, value); }
        }

        /// <summary>
        /// DP for <see cref="CloseOnEscapeKey"/>.
        /// </summary>
        public static readonly DependencyProperty CloseOnEscapeKeyProperty =
            DependencyProperty.Register("CloseOnEscapeKey", typeof(bool), typeof(DialogControl), new PropertyMetadata(true));



        #endregion

        #region methods

        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.Input.Keyboard.PreviewKeyDown" /> attached event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.KeyEventArgs" /> that contains the event data.</param>
        protected override void OnPreviewKeyDown(System.Windows.Input.KeyEventArgs e)
        {
            if (CloseOnEscapeKey && e.Key == System.Windows.Input.Key.Escape)
            {
                e.Handled = true;
                DialogResult = false;
            }
            base.OnPreviewKeyDown(e);
        }

        internal void TryFocus()
        {
            // if subclass doesn't override OnFocus then try to find a focusable control ourselves.
            var focusMethod = this.GetType().GetMethod("OnFocus", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (focusMethod != null && focusMethod.DeclaringType != typeof(DialogControl))
            {
                this.ProcessInVisualTree<FrameworkElement>(fe =>
                {
                    var matches = fe.Visibility == System.Windows.Visibility.Visible && fe.Focusable;
                    if (matches)
                    {
                        fe.Focus();
                    }
                    return matches;
                });
            }
            else
            {
                OnFocus();
            }
        }

        /// <summary>
        /// Called when dialog has been shown and focus needs to happen.
        /// </summary>
        protected virtual void OnFocus() { }

        /// <summary>
        /// Shows the dialog.
        /// </summary>
        /// <param name="window">The window.</param>
        public void ShowDialog(Window window)
        {
            ShowDialog(window.FindInVisualTree<DialogControlContainer>());
        }

        /// <summary>
        /// Shows the dialog.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <exception cref="System.ArgumentNullException">container</exception>
        /// <exception cref="System.ArgumentException">This dialog already has a container.</exception>
        public void ShowDialog(DialogControlContainer container)
        {
            if (container == null) { throw new ArgumentNullException("container"); }
            if (this.Container != null && this.Container != container) { throw new ArgumentException("This dialog already has a container."); }

            this.Container = container;

            container.Show(this);
        }

        #endregion
    }
}
