using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace ModernWPF.Controls
{
    // modified from http://matthiasshapiro.com/2009/05/06/how-to-create-an-animated-scrollviewer-or-listbox-in-wpf/

    /// <summary>
    /// A scroll viewer that will animate where possible (currently on mouse wheel + click only).
    /// </summary>
    [TemplatePart(Name = "PART_AniVerticalScrollBar", Type = typeof(ScrollBar))]
    [TemplatePart(Name = "PART_AniHorizontalScrollBar", Type = typeof(ScrollBar))]
    public class AnimatedScrollViewer : ScrollViewer
    {
        static AnimatedScrollViewer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(AnimatedScrollViewer), new FrameworkPropertyMetadata(typeof(AnimatedScrollViewer)));
        }   
        
        ScrollBar _aniVerticalScrollBar;
        ScrollBar _aniHorizontalScrollBar;

        #region ScrollViewer Override Methods

        /// <summary>
        /// Called when an internal process or application calls <see cref="M:System.Windows.FrameworkElement.ApplyTemplate" />, which is used to build the current template's visual tree.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            if (DesignerProperties.GetIsInDesignMode(this)) { return; }

            ScrollBar aniVScroll = base.GetTemplateChild("PART_AniVerticalScrollBar") as ScrollBar;
            if (aniVScroll != null)
            {
                _aniVerticalScrollBar = aniVScroll;
            }
            _aniVerticalScrollBar.ValueChanged += new RoutedPropertyChangedEventHandler<double>(VScrollBar_ValueChanged);

            ScrollBar aniHScroll = base.GetTemplateChild("PART_AniHorizontalScrollBar") as ScrollBar;
            if (aniHScroll != null)
            {
                _aniHorizontalScrollBar = aniHScroll;
            }
            _aniHorizontalScrollBar.ValueChanged += new RoutedPropertyChangedEventHandler<double>(HScrollBar_ValueChanged);
        }


        const int smallStep = 16;
        const int largeStep = 48;
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
                        TargetVerticalOffset = Math.Min(ScrollableHeight, VerticalOffset + largeStep);
                        e.Handled = true;
                    }
                    else if (this.CanHScrollRight())
                    {
                        TargetHorizontalOffset = Math.Min(ScrollableWidth, HorizontalOffset + largeStep);
                        e.Handled = true;
                    }
                }
                else
                {
                    if (this.CanVScrollUp())
                    {
                        TargetVerticalOffset = Math.Max(0, VerticalOffset - largeStep);
                        e.Handled = true;
                    }
                    else if (this.CanHScrollLeft())
                    {
                        TargetHorizontalOffset = Math.Max(0, HorizontalOffset - largeStep);
                        e.Handled = true;
                    }
                }
                if (e.Handled)
                {
                    AnimateNow();
                }
            }

            base.OnPreviewMouseWheel(e);
        }

        //protected override void OnKeyDown(KeyEventArgs e)
        //{
        //    if (CanKeyboardScroll)
        //    {
        //        Key keyPressed = e.Key;
        //        double newVerticalPos = TargetVerticalOffset;
        //        double newHorizontalPos = TargetHorizontalOffset;
        //        bool isKeyHandled = false;

        //        //Vertical Key Strokes code
        //        if (keyPressed == Key.Down)
        //        {
        //            newVerticalPos = NormalizeScrollPos((newVerticalPos + 16.0), Orientation.Vertical);
        //            isKeyHandled = true;
        //        }
        //        else if (keyPressed == Key.PageDown)
        //        {
        //            newVerticalPos = NormalizeScrollPos((newVerticalPos + ViewportHeight), Orientation.Vertical);
        //            isKeyHandled = true;
        //        }
        //        else if (keyPressed == Key.Up)
        //        {
        //            newVerticalPos = NormalizeScrollPos((newVerticalPos - 16.0), Orientation.Vertical);
        //            isKeyHandled = true;
        //        }
        //        else if (keyPressed == Key.PageUp)
        //        {
        //            newVerticalPos = NormalizeScrollPos((newVerticalPos - ViewportHeight), Orientation.Vertical);
        //            isKeyHandled = true;
        //        }

        //        if (newVerticalPos != TargetVerticalOffset)
        //        {
        //            TargetVerticalOffset = newVerticalPos;
        //        }

        //        //Horizontal Key Strokes Code

        //        if (keyPressed == Key.Right)
        //        {
        //            newHorizontalPos = NormalizeScrollPos((newHorizontalPos + 16), Orientation.Horizontal);
        //            isKeyHandled = true;
        //        }
        //        else if (keyPressed == Key.Left)
        //        {
        //            newHorizontalPos = NormalizeScrollPos((newHorizontalPos - 16), Orientation.Horizontal);
        //            isKeyHandled = true;
        //        }

        //        if (newHorizontalPos != TargetHorizontalOffset)
        //        {
        //            TargetHorizontalOffset = newHorizontalPos;
        //        }

        //        e.Handled = isKeyHandled;
        //    }
        //    base.OnKeyDown(e);
        //}

        //private double NormalizeScrollPos(AnimatedScrollViewer thisScroll, double scrollChange, Orientation o)
        //{
        //    double returnValue = scrollChange;

        //    if (scrollChange < 0)
        //    {
        //        returnValue = 0;
        //    }

        //    if (o == Orientation.Vertical && scrollChange > thisScroll.ScrollableHeight)
        //    {
        //        returnValue = thisScroll.ScrollableHeight;
        //    }
        //    else if (o == Orientation.Horizontal && scrollChange > thisScroll.ScrollableWidth)
        //    {
        //        returnValue = thisScroll.ScrollableWidth;
        //    }

        //    return returnValue;
        //}


        #endregion

        #region Custom Event Handlers

        void VScrollBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            double oldTargetVOffset = (double)e.OldValue;
            double newTargetVOffset = (double)e.NewValue;

            if (newTargetVOffset != TargetVerticalOffset)
            {
                double deltaVOffset = Math.Round((newTargetVOffset - oldTargetVOffset), 3);

                if (deltaVOffset == 1)
                {
                    TargetVerticalOffset = oldTargetVOffset + ViewportHeight;

                }
                else if (deltaVOffset == -1)
                {
                    TargetVerticalOffset = oldTargetVOffset - ViewportHeight;
                }
                else if (deltaVOffset == 0.1)
                {
                    TargetVerticalOffset = oldTargetVOffset + 16.0;
                }
                else if (deltaVOffset == -0.1)
                {
                    TargetVerticalOffset = oldTargetVOffset - 16.0;
                }
                else
                {
                    TargetVerticalOffset = newTargetVOffset;
                }
            }
        }

        void HScrollBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            double oldTargetHOffset = (double)e.OldValue;
            double newTargetHOffset = (double)e.NewValue;

            if (newTargetHOffset != TargetHorizontalOffset)
            {

                double deltaVOffset = Math.Round((newTargetHOffset - oldTargetHOffset), 3);

                if (deltaVOffset == 1)
                {
                    TargetHorizontalOffset = oldTargetHOffset + ViewportWidth;

                }
                else if (deltaVOffset == -1)
                {
                    TargetHorizontalOffset = oldTargetHOffset - ViewportWidth;
                }
                else if (deltaVOffset == 0.1)
                {
                    TargetHorizontalOffset = oldTargetHOffset + 16.0;
                }
                else if (deltaVOffset == -0.1)
                {
                    TargetHorizontalOffset = oldTargetHOffset - 16.0;
                }
                else
                {
                    TargetHorizontalOffset = newTargetHOffset;
                }
            }
        }

        #endregion

        #region Custom Dependency Properties

        #region TargetVerticalOffset (DependencyProperty)(double)

        /// <summary>
        /// This is the VerticalOffset that we'd like to animate to
        /// </summary>
        public double TargetVerticalOffset
        {
            get { return (double)GetValue(TargetVerticalOffsetProperty); }
            set { SetValue(TargetVerticalOffsetProperty, value); }
        }
        /// <summary>
        /// DP for <see cref="TargetVerticalOffset"/>.
        /// </summary>
        public static readonly DependencyProperty TargetVerticalOffsetProperty =
            DependencyProperty.Register("TargetVerticalOffset", typeof(double), typeof(AnimatedScrollViewer),
            new PropertyMetadata(0.0, new PropertyChangedCallback(OnTargetVerticalOffsetChanged)));

        private static void OnTargetVerticalOffsetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            AnimatedScrollViewer thisScroller = (AnimatedScrollViewer)d;

            if ((double)e.NewValue != thisScroller._aniVerticalScrollBar.Value)
            {
                thisScroller._aniVerticalScrollBar.Value = (double)e.NewValue;
            }

            thisScroller.AnimateNow();
        }

        #endregion

        #region TargetHorizontalOffset (DependencyProperty) (double)

        /// <summary>
        /// This is the HorizontalOffset that we'll be animating to
        /// </summary>
        public double TargetHorizontalOffset
        {
            get { return (double)GetValue(TargetHorizontalOffsetProperty); }
            set { SetValue(TargetHorizontalOffsetProperty, value); }
        }
        /// <summary>
        /// DP for <see cref="TargetHorizontalOffset"/>.
        /// </summary>
        public static readonly DependencyProperty TargetHorizontalOffsetProperty =
            DependencyProperty.Register("TargetHorizontalOffset", typeof(double), typeof(AnimatedScrollViewer),
            new PropertyMetadata(0.0, new PropertyChangedCallback(OnTargetHorizontalOffsetChanged)));

        private static void OnTargetHorizontalOffsetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            AnimatedScrollViewer thisScroller = (AnimatedScrollViewer)d;

            if ((double)e.NewValue != thisScroller._aniHorizontalScrollBar.Value)
            {
                thisScroller._aniHorizontalScrollBar.Value = (double)e.NewValue;
            }

            thisScroller.AnimateNow();
        }

        #endregion

        #region HorizontalScrollOffset (DependencyProperty) (double)

        /// <summary>
        /// This is the actual horizontal offset property we're going use as an animation helper
        /// </summary>
        public double HorizontalScrollOffset
        {
            get { return (double)GetValue(HorizontalScrollOffsetProperty); }
            set { SetValue(HorizontalScrollOffsetProperty, value); }
        }
        /// <summary>
        /// DP for <see cref="HorizontalScrollOffset"/>.
        /// </summary>
        public static readonly DependencyProperty HorizontalScrollOffsetProperty =
            DependencyProperty.Register("HorizontalScrollOffset", typeof(double), typeof(AnimatedScrollViewer),
            new PropertyMetadata(0.0, new PropertyChangedCallback(OnHorizontalScrollOffsetChanged)));

        private static void OnHorizontalScrollOffsetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            AnimatedScrollViewer thisSViewer = (AnimatedScrollViewer)d;
            thisSViewer.ScrollToHorizontalOffset((double)e.NewValue);
        }

        #endregion

        #region VerticalScrollOffset (DependencyProperty) (double)

        /// <summary>
        /// This is the actual VerticalOffset we're going to use as an animation helper
        /// </summary>
        public double VerticalScrollOffset
        {
            get { return (double)GetValue(VerticalScrollOffsetProperty); }
            set { SetValue(VerticalScrollOffsetProperty, value); }
        }
        /// <summary>
        /// DP for <see cref="VerticalScrollOffset"/>.
        /// </summary>
        public static readonly DependencyProperty VerticalScrollOffsetProperty =
            DependencyProperty.Register("VerticalScrollOffset", typeof(double), typeof(AnimatedScrollViewer),
            new PropertyMetadata(0.0, new PropertyChangedCallback(OnVerticalScrollOffsetChanged)));

        private static void OnVerticalScrollOffsetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            AnimatedScrollViewer thisSViewer = (AnimatedScrollViewer)d;
            thisSViewer.ScrollToVerticalOffset((double)e.NewValue);
        }

        #endregion


        #region Animation properties

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



        /// <summary>
        /// A property for changing the time it takes to scroll to a new
        /// position.
        /// </summary>
        /// <value>
        /// The duration to animate.
        /// </value>
        public TimeSpan AnimateDuration
        {
            get { return (TimeSpan)GetValue(AnimateDurationProperty); }
            set { SetValue(AnimateDurationProperty, value); }
        }

        /// <summary>
        /// DP for <see cref="AnimateDuration"/>.
        /// </summary>
        public static readonly DependencyProperty AnimateDurationProperty =
            DependencyProperty.Register("AnimateDuration", typeof(TimeSpan), typeof(AnimatedScrollViewer), new PropertyMetadata(new TimeSpan(0, 0, 0, 0, 250)));

        /// <summary>
        /// A property to allow users to describe a custom spline for the scrolling animation.
        /// </summary>
        public KeySpline AnimateSpline
        {
            get { return (KeySpline)GetValue(AnimateSplineProperty); }
            set { SetValue(AnimateSplineProperty, value); }
        }

        public static readonly DependencyProperty AnimateSplineProperty =
            DependencyProperty.Register("AnimateSpline", typeof(KeySpline), typeof(AnimatedScrollViewer),
              new PropertyMetadata(new KeySpline(0.024, 0.914, 0.717, 1)));

        #endregion

        #region CanKeyboardScroll (Dependency Property)

        //public static readonly DependencyProperty CanKeyboardScrollProperty =
        //    DependencyProperty.Register("CanKeyboardScroll", typeof(bool), typeof(AnimatedScrollViewer),
        //        new FrameworkPropertyMetadata((bool)true));

        //public bool CanKeyboardScroll
        //{
        //    get { return (bool)GetValue(CanKeyboardScrollProperty); }
        //    set { SetValue(CanKeyboardScrollProperty, value); }
        //}

        #endregion



        #endregion

        private void AnimateNow()
        {
            if (AnimateScroll && !SystemParameters.IsRemoteSession)
            {
                Duration targetTime = new Duration(AnimateDuration);
                KeyTime targetKeyTime = AnimateDuration;
                KeySpline targetKeySpline = AnimateSpline;

                DoubleAnimationUsingKeyFrames animateHScrollKeyFramed = new DoubleAnimationUsingKeyFrames();
                DoubleAnimationUsingKeyFrames animateVScrollKeyFramed = new DoubleAnimationUsingKeyFrames();

                SplineDoubleKeyFrame HScrollKey1 = new SplineDoubleKeyFrame(TargetHorizontalOffset, targetKeyTime, targetKeySpline);
                SplineDoubleKeyFrame VScrollKey1 = new SplineDoubleKeyFrame(TargetVerticalOffset, targetKeyTime, targetKeySpline);
                animateHScrollKeyFramed.KeyFrames.Add(HScrollKey1);
                animateVScrollKeyFramed.KeyFrames.Add(VScrollKey1);

                BeginAnimation(HorizontalScrollOffsetProperty, animateHScrollKeyFramed);
                BeginAnimation(VerticalScrollOffsetProperty, animateVScrollKeyFramed);

                //CommandBindingCollection testCollection = CommandBindings;
                //int blah = testCollection.Count;
            }
            else
            {
                HorizontalScrollOffset = TargetHorizontalOffset;
                VerticalScrollOffset = TargetVerticalOffset;
            }

        }

        #region my old stuff

        //#region animation parts

        //bool _isVanimating;
        //bool _isHanimating;

        //private void AnimateHorizontal(double newOffset)
        //{
        //    if (_isHanimating) { return; }
        //    Debug.WriteLine("Scroll horizontal to " + newOffset);
        //    if (!AnimateScroll || SystemParameters.IsRemoteSession)
        //    {
        //        BindableHorizontalOffset = newOffset;
        //    }
        //    else
        //    {
        //        _isHanimating = true;
        //        var hStory = new Storyboard();
        //        hStory.Completed += (s, e) => { _isHanimating = false; };
        //        var hAnime = CreateDoubleAnimation(BindableHorizontalOffsetProperty);
        //        hStory.Children.Add(hAnime);
        //        hAnime.To = newOffset;
        //        BeginStoryboard(hStory, HandoffBehavior.SnapshotAndReplace);
        //    }
        //}

        //private void AnimateVertical(double newOffset)
        //{
        //    if (_isVanimating) { return; }
        //    Debug.WriteLine("Scroll vertical to " + newOffset);
        //    if (!AnimateScroll || SystemParameters.IsRemoteSession)
        //    {
        //        BindableVerticalOffset = newOffset;
        //    }
        //    else
        //    {
        //        _isVanimating = true;
        //        var vStory = new Storyboard();
        //        vStory.Completed += (s, e) => { _isVanimating = false; };
        //        var vAnime = CreateDoubleAnimation(BindableVerticalOffsetProperty);
        //        vStory.Children.Add(vAnime);
        //        vAnime.To = newOffset;
        //        BeginStoryboard(vStory, HandoffBehavior.SnapshotAndReplace);
        //    }
        //}

        //private DoubleAnimation CreateDoubleAnimation(DependencyProperty property)
        //{
        //    DoubleAnimation da = new DoubleAnimation
        //    {
        //        Duration = TimeSpan.FromSeconds(.25),
        //        //EasingFunction = new QuinticEase() { EasingMode = EasingMode.EaseOut }
        //        EasingFunction = new QuarticEase() { EasingMode = EasingMode.EaseOut }
        //    };
        //    Storyboard.SetTargetProperty(da, new PropertyPath(property));
        //    return da;
        //}

        //#endregion

        #endregion



    }
}
