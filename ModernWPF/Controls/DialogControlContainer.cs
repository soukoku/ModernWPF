using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace ModernWPF.Controls
{
    /// <summary>
    /// A container element for hosting <see cref="DialogControl"/>.
    /// </summary>
    public class DialogControlContainer : ContentControl
    {
        static DialogControlContainer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DialogControlContainer), new FrameworkPropertyMetadata(typeof(DialogControlContainer)));
        }

        #region properties



        /// <summary>
        /// Gets a value indicating whether this container has any dialog open.
        /// </summary>
        /// <value>
        ///   <c>true</c> if it has dialog open; otherwise, <c>false</c>.
        /// </value>
        public bool HasDialogOpen
        {
            get { return (bool)GetValue(HasDialogOpenProperty); }
            private set
            {
                var changed = value != HasDialogOpen;
                SetValue(HasDialogOpenProperty, value);
                if (changed)
                {
                    if (value)
                    {
                        VisualStateManager.GoToState(this, "IsOpen", !SystemParameters.IsRemoteSession);
                    }
                    else
                    {
                        VisualStateManager.GoToState(this, "IsClosed", !SystemParameters.IsRemoteSession);
                    }
                }
            }
        }

        /// <summary>
        /// DP for <see cref="HasDialogOpen"/>.
        /// </summary>
        static readonly DependencyProperty HasDialogOpenProperty =
            DependencyProperty.Register("HasDialogOpen", typeof(bool), typeof(DialogControlContainer), new PropertyMetadata(false));


        /// <summary>
        /// Gets or sets the target to disable when dialogs are visible.
        /// </summary>
        /// <value>
        /// The disable target.
        /// </value>
        public FrameworkElement DisableTarget
        {
            get { return (FrameworkElement)GetValue(DisableTargetProperty); }
            set { SetValue(DisableTargetProperty, value); }
        }

        /// <summary>
        /// DP for <see cref="DisableTarget"/>.
        /// </summary>
        public static readonly DependencyProperty DisableTargetProperty =
            DependencyProperty.Register("DisableTarget", typeof(FrameworkElement), typeof(DialogControlContainer), new PropertyMetadata(null));




        #endregion

        object _openLock = new object();
        List<DialogControl> _openDialogs = new List<DialogControl>();

        internal void Show(DialogControl dialog)
        {
            lock (_openLock)
            {
                if (dialog.Container != null)
                {
                    // already somewhere in this stack
                    _openDialogs.Remove(dialog);
                }
                _openDialogs.Add(dialog);
                ShowMostRecentDialogIfNecessary();
            }
        }

        private void ShowMostRecentDialogIfNecessary()
        {
            var next = _openDialogs.LastOrDefault();
            if (next == null)
            {
                HasDialogOpen = false;
                this.Content = null;
                if (DisableTarget != null) { DisableTarget.IsEnabled = true; }
            }
            else
            {
                if (DisableTarget != null) { DisableTarget.IsEnabled = false; }
                this.Content = next;
                HasDialogOpen = true;

                var dt = new DispatcherTimer(DispatcherPriority.Send);
                dt.Tick += (s, e) =>
                {
                    dt.Stop();
                    next.TryFocus();
                };
                dt.Interval = TimeSpan.FromMilliseconds(300);
                dt.Start();

            }
        }

        internal void Close(DialogControl dialog)
        {
            lock (_openLock)
            {
                dialog.Container = null;
                _openDialogs.Remove(dialog);
                ShowMostRecentDialogIfNecessary();
            }
        }
    }
}
