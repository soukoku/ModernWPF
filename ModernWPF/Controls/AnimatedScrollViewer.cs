using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace ModernWPF.Controls
{

    /// <summary>
    /// A scroll viewer that will animate where possible (currently on mouse wheel only).
    /// </summary>
    public class AnimatedScrollViewer : ScrollViewer
    {

        #region properties

        /// <summary>
        /// Gets or sets the bindable vertical offset.
        /// </summary>
        /// <value>
        /// The bindable vertical offset.
        /// </value>
        public double BindableVerticalOffset
        {
            get { return (double)this.GetValue(BindableVerticalOffsetProperty); }
            set { this.SetValue(BindableVerticalOffsetProperty, value); }
        }

        /// <summary>
        /// DP for <see cref="BindableVerticalOffset"/>.
        /// </summary>
        public static DependencyProperty BindableVerticalOffsetProperty =
            DependencyProperty.Register("BindableVerticalOffset", typeof(double), typeof(AnimatedScrollViewer), new PropertyMetadata(0d, VerticalChanged));

        private static void VerticalChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            AnimatedScrollViewer viewer = d as AnimatedScrollViewer;
            viewer.ScrollToVerticalOffset((double)e.NewValue);
        }

        /// <summary>
        /// Gets or sets the bindable horizontal offset.
        /// </summary>
        /// <value>
        /// The bindable horizontal offset.
        /// </value>
        public double BindableHorizontalOffset
        {
            get { return (double)this.GetValue(BindableHorizontalOffsetProperty); }
            set { this.SetValue(BindableHorizontalOffsetProperty, value); }
        }

        /// <summary>
        /// DP for <see cref="BindableHorizontalOffset"/>.
        /// </summary>
        public static DependencyProperty BindableHorizontalOffsetProperty =
            DependencyProperty.Register("BindableHorizontalOffsetOffset", typeof(double), typeof(AnimatedScrollViewer), new PropertyMetadata(0d, HorizontalChanged));

        private static void HorizontalChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            AnimatedScrollViewer viewer = d as AnimatedScrollViewer;
            viewer.ScrollToHorizontalOffset((double)e.NewValue);
        }

        /// <summary>
        /// Gets or sets a value indicating whether scrolling will be animated (if supported).
        /// </summary>
        /// <value>
        ///   <c>true</c> if to animate scrolling; otherwise, <c>false</c>.
        /// </value>
        public bool AnimateScroll
        {
            get { return (bool)GetValue(AnimateScrollProperty); }
            set { SetValue(AnimateScrollProperty, value); }
        }

        /// <summary>
        /// DP for <see cref="AnimateScroll"/>.
        /// </summary>
        public static readonly DependencyProperty AnimateScrollProperty =
            DependencyProperty.Register("AnimateScroll", typeof(bool), typeof(AnimatedScrollViewer), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.Inherits));



        #endregion

        Storyboard _hStory;
        DoubleAnimation _hAnime;
        Storyboard _vStory;
        DoubleAnimation _vAnime;

        /// <summary>
        /// Initializes a new instance of the <see cref="AnimatedScrollViewer"/> class.
        /// </summary>
        public AnimatedScrollViewer()
        {
            _hStory = new Storyboard();
            _vStory = new Storyboard();

            _hAnime = CreateDoubleAnimation(BindableHorizontalOffsetProperty);
            _vAnime = CreateDoubleAnimation(BindableVerticalOffsetProperty);

            _hStory.Children.Add(_hAnime);
            _vStory.Children.Add(_vAnime);
        }

        private DoubleAnimation CreateDoubleAnimation(DependencyProperty property)
        {
            DoubleAnimation da = new DoubleAnimation
            {
                Duration = TimeSpan.FromSeconds(.25),
                //EasingFunction = new QuinticEase() { EasingMode = EasingMode.EaseOut }
                EasingFunction = new QuarticEase() { EasingMode = EasingMode.EaseOut }
            };
            //Storyboard.SetTarget(da, this);
            Storyboard.SetTargetProperty(da, new PropertyPath(property));
            return da;
        }

        int GetStepSize()
        {
            return 48;
        }

        // cannot use OnMouseWheel since it never gets called.

        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.Input.Mouse.PreviewMouseWheel" /> attached event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseWheelEventArgs" /> that contains the event data.</param>
        protected override void OnPreviewMouseWheel(MouseWheelEventArgs e)
        {
            if (!e.Handled)
            {
                if (e.Delta < 0)
                {
                    if (this.CanVScrollDown())
                    {
                        AnimateVertical(Math.Min(ScrollableHeight, VerticalOffset + GetStepSize()));
                        e.Handled = true;
                    }
                    else if (this.CanHScrollRight())
                    {
                        AnimateHorizontal(Math.Min(ScrollableWidth, HorizontalOffset + GetStepSize()));
                        e.Handled = true;
                    }
                }
                else
                {
                    if (this.CanVScrollUp())
                    {
                        AnimateVertical(Math.Max(0, VerticalOffset - GetStepSize()));
                        e.Handled = true;
                    }
                    else if (this.CanHScrollLeft())
                    {
                        AnimateHorizontal(Math.Max(0, HorizontalOffset - GetStepSize()));
                        e.Handled = true;
                    }
                }
            }
            base.OnPreviewMouseWheel(e);
        }

        private void AnimateHorizontal(double newOffset)
        {
            Debug.WriteLine("Scroll horizontal to " + newOffset);
            if (!AnimateScroll || SystemParameters.IsRemoteSession)
            {
                BindableHorizontalOffset = newOffset;
            }
            else
            {
                _hAnime.To = newOffset;
                BeginStoryboard(_hStory, HandoffBehavior.SnapshotAndReplace);
            }
        }

        private void AnimateVertical(double newOffset)
        {
            Debug.WriteLine("Scroll vertical to " + newOffset);
            if (!AnimateScroll || SystemParameters.IsRemoteSession)
            {
                BindableVerticalOffset = newOffset;
            }
            else
            {
                _vAnime.To = newOffset;
                BeginStoryboard(_vStory, HandoffBehavior.SnapshotAndReplace);
            }
        }


    }
}
