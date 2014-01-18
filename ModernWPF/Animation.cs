using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace ModernWPF
{
    /// <summary>
    /// Quick utility for doing simple ad-hoc animations.
    /// </summary>
    public static class Animation
    {
        static Animation()
        {
            TypicalDuration = TimeSpan.FromMilliseconds(250);
            TypicalEasing = new QuarticEase { EasingMode = EasingMode.EaseOut };
            TypicalEasing.Freeze();
        }

        /// <summary>
        /// Gets a flag indicating whether animation should be used.
        /// This does not stop controls from using animations in this class.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the app should animate; otherwise, <c>false</c>.
        /// </value>
        public static bool ShouldAnimate
        {
            get
            {
                return !SystemParameters.IsRemoteSession || (RenderCapability.Tier >> 16) > 0;
            }
        }

        /// <summary>
        /// Gets the a typical animation duration.
        /// </summary>
        public static TimeSpan TypicalDuration { get; private set; }

        /// <summary>
        /// Gets the typical easing function.
        /// </summary>
        /// <value>
        /// The typical easing.
        /// </value>
        public static QuarticEase TypicalEasing { get; private set; }

        /// <summary>
        /// Gets the typical slide offset value. Default is 15.
        /// </summary>
        /// <value>
        /// The typical slide offset.
        /// </value>
        public static double TypicalSlideOffset { get { return 15; } }

        /// <summary>
        /// Slides the element in with translate transform.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="duration">The duration.</param>
        public static void SlideIn(UIElement element, TimeSpan duration)
        {
            SlideIn(element, duration, TypicalSlideOffset, TypicalEasing);
        }

        /// <summary>
        /// Slides the element in with translate transform.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="duration">The duration.</param>
        /// <param name="startOffset">The start offset.</param>
        public static void SlideIn(UIElement element, TimeSpan duration, double startOffset)
        {
            SlideIn(element, duration, startOffset, TypicalEasing);
        }

        /// <summary>
        /// Slides the element in with translate transform.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="duration">The duration.</param>
        /// <param name="startOffset">The start offset.</param>
        /// <param name="easing">The easing.</param>
        public static void SlideIn(UIElement element, TimeSpan duration, double startOffset, IEasingFunction easing)
        {
            SlideIn(element, duration, startOffset, easing, SlideFromDirection.Left);
        }

        /// <summary>
        /// Slides the element in with translate transform
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="duration">The duration.</param>
        /// <param name="startOffset">The start offset.</param>
        /// <param name="easing">The easing.</param>
        /// <param name="direction">The direction.</param>
        public static void SlideIn(UIElement element, TimeSpan duration, double startOffset, IEasingFunction easing, SlideFromDirection direction)
        {
            if (element == null) { return; }

            var da = new DoubleAnimation();
            da.Duration = duration;
            da.EasingFunction = easing;
            da.To = 0;

            TranslateTransform transform = FindOrCreateRenderXform(element);
            switch (direction)
            {
                case SlideFromDirection.Top:
                    da.From = -startOffset;
                    transform.BeginAnimation(TranslateTransform.YProperty, da);
                    break;
                case SlideFromDirection.Left:
                    da.From = -startOffset;
                    transform.BeginAnimation(TranslateTransform.XProperty, da);
                    break;
                case SlideFromDirection.Right:
                    da.From = startOffset;
                    transform.BeginAnimation(TranslateTransform.XProperty, da);
                    break;
                case SlideFromDirection.Bottom:
                    da.From = startOffset;
                    transform.BeginAnimation(TranslateTransform.YProperty, da);
                    break;
            }
        }

        private static TranslateTransform FindOrCreateRenderXform(UIElement element)
        {
            TranslateTransform transform = null;
            if (element.RenderTransform != null)
            {
                var grp = element.RenderTransform as TransformGroup;
                if (grp == null)
                {
                    var hit = grp.Children.FirstOrDefault(t => t is TranslateTransform);
                    transform = (TranslateTransform)hit;
                }
                else
                {
                    transform = element.RenderTransform as TranslateTransform;
                }
            }

            if (transform == null)
            {
                transform = new TranslateTransform();
                // probably shouldn't replace existing transform but anyway
                element.RenderTransform = transform;
            }
            return transform;
        }

        /// <summary>
        /// Indicates the slide direction.
        /// </summary>
        public enum SlideFromDirection
        {
            /// <summary>
            /// Slide from the left.
            /// </summary>
            Left,
            /// <summary>
            /// Slide from the top.
            /// </summary>
            Top,
            /// <summary>
            /// Slide from the right.
            /// </summary>
            Right,
            /// <summary>
            /// Slide from the bottom.
            /// </summary>
            Bottom
        }
    }
}
