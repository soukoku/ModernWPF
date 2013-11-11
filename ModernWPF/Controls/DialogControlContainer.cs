﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace ModernWPF.Controls
{
    /// <summary>
    /// A container element for hosting <see cref="DialogControl"/>.
    /// </summary>
    [TemplatePart(Name = PARTContent, Type = typeof(ContentPresenter))]
    public class DialogControlContainer : ContentControl
    {
        static DialogControlContainer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DialogControlContainer), new FrameworkPropertyMetadata(typeof(DialogControlContainer)));

        }
        const string PARTContent = "PART_Content";

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
        /// The dependency property for <see cref="HasDialogOpen"/>.
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
        /// The dependency property for <see cref="DisableTarget"/>.
        /// </summary>
        public static readonly DependencyProperty DisableTargetProperty =
            DependencyProperty.Register("DisableTarget", typeof(FrameworkElement), typeof(DialogControlContainer), new PropertyMetadata(null));




        #endregion

        ContentPresenter _presenter;
        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes call <see cref="M:System.Windows.FrameworkElement.ApplyTemplate" />.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _presenter = GetTemplateChild(PARTContent) as ContentPresenter;
        }


        object _openLock = new object();
        List<DialogControl> _openDialogs = new List<DialogControl>();

        internal void Close(DialogControl dialog)
        {
            lock (_openLock)
            {
                dialog.Container = null;
                _openDialogs.Remove(dialog);
                ShowMostRecentDialogIfNecessary();
            }
        }

        internal void Show(DialogControl dialog)
        {
            if (dialog.Container != null && dialog.Container != this) { throw new ArgumentException("This dialog already has a container.", "dialog"); }

            if (Content == dialog) { return; }

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
                if (_presenter != null) { BindingOperations.ClearAllBindings(_presenter); }
                if (DisableTarget != null) { DisableTarget.IsEnabled = true; }
            }
            else
            {
                next.Container = this;
                if (DisableTarget != null) { DisableTarget.IsEnabled = !next.DisableTarget; }
                if (_presenter != null)
                {
                    BindContentAlignment(next);
                }
                this.Content = next;
                if (!SystemParameters.IsRemoteSession)
                {
                    DoShowContentAnimation(next);
                }
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

        private void BindContentAlignment(DialogControl content)
        {
            var hbind = new Binding(HorizontalAlignmentProperty.Name);
            hbind.Source = content;
            hbind.NotifyOnSourceUpdated = true;
            BindingOperations.SetBinding(_presenter, HorizontalAlignmentProperty, hbind);


            var vbind = new Binding(VerticalAlignmentProperty.Name);
            vbind.Source = content;
            vbind.NotifyOnSourceUpdated = true;
            BindingOperations.SetBinding(_presenter, VerticalAlignmentProperty, vbind);
        }

        void DoShowContentAnimation(DialogControl content)
        {
            var da = new DoubleAnimation();
            da.Duration = TimeSpan.FromMilliseconds(250);
            da.EasingFunction = new QuarticEase() { EasingMode = EasingMode.EaseOut };
            da.To = 0;

            var dir = DetermineAniDirection(content);
            TranslateTransform transform = new TranslateTransform();
            _presenter.RenderTransform = transform;
            switch (dir)
            {
                case AniFromDirection.Top:
                    da.From = -200;
                    transform.BeginAnimation(TranslateTransform.YProperty, da);
                    break;
                case AniFromDirection.Left:
                    da.From = -200;
                    transform.BeginAnimation(TranslateTransform.XProperty, da);
                    break;
                case AniFromDirection.Right:
                    da.From = 200;
                    transform.BeginAnimation(TranslateTransform.XProperty, da);
                    break;
                case AniFromDirection.Bottom:
                    da.From = 200;
                    transform.BeginAnimation(TranslateTransform.YProperty, da);
                    break;
            }
        }

        AniFromDirection DetermineAniDirection(DialogControl content)
        { 
            if (content != null)
            {
                if (content.VerticalAlignment == System.Windows.VerticalAlignment.Stretch)
                {
                    switch (content.HorizontalAlignment)
                    {
                        case System.Windows.HorizontalAlignment.Left:
                            return AniFromDirection.Left;
                        case System.Windows.HorizontalAlignment.Right:
                            return AniFromDirection.Right;
                    }
                }
                else if (content.HorizontalAlignment == System.Windows.HorizontalAlignment.Stretch &&
                    content.VerticalAlignment == System.Windows.VerticalAlignment.Bottom)
                {
                    return AniFromDirection.Bottom;
                }
            }
            return AniFromDirection.Top;
        }

        enum AniFromDirection
        {
            Left,
            Top,
            Right,
            Bottom
        }
    }
}
