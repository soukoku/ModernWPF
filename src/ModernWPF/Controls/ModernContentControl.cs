using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace ModernWPF.Controls
{
    // originally from http://xamlcoder.com/blog/2010/11/04/creating-a-metro-ui-style-control/
    // but no longer resembles it

    /// <summary>
    /// A <see cref="ContentControl" /> that animates content in.
    /// </summary>
    //[TemplatePart(Name = PARTContent, Type = typeof(ContentPresenter))]
    public class ModernContentControl : ContentControl
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline")]
        static ModernContentControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ModernContentControl), new FrameworkPropertyMetadata(typeof(ModernContentControl)));
        }

        //const string PARTContent = "PART_Content";

        //ContentPresenter _presenter;

        /// <summary>
        /// Initializes a new instance of the <see cref="ModernContentControl"/> class.
        /// </summary>
        public ModernContentControl()
        {
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                this.Loaded += (s, e) =>
                {
                    AnimateIn();
                };
                this.DataContextChanged += (s, e) =>
                {
                    AnimateIn();
                };
                //this.Unloaded += (s, e) =>
                //{
                //    AnimateOut();
                //};
            }
        }

        ///// <summary>
        ///// Animates the content out.
        ///// </summary>
        //public void AnimateOut()
        //{
        //    if (Animation.ShouldAnimate)
        //    {
        //        Animation.FadeOut(this, Animation.TypicalDuration);
        //    }
        //}

        /// <summary>
        /// Animates the content in.
        /// </summary>
        public void AnimateIn()
        {
            if (Animation.ShouldAnimate)
            {
                Animation.FadeIn(this, Animation.TypicalDuration);
                Animation.SlideIn(this, SlideFromDirection, TimeSpan.FromMilliseconds((double)AnimationSpeed.Slow));
            }
        }

        ///// <summary>
        ///// When overridden in a derived class, is invoked whenever application code or internal processes call <see cref="M:System.Windows.FrameworkElement.ApplyTemplate" />.
        ///// </summary>
        //public override void OnApplyTemplate()
        //{
        //    base.OnApplyTemplate();
        //    _presenter = GetTemplateChild(PARTContent) as ContentPresenter;
        //}


        /// <summary>
        /// Gets or sets the slide from animation direction.
        /// </summary>
        /// <value>
        /// The slide from direction.
        /// </value>
        public SlideFromDirection SlideFromDirection
        {
            get { return (SlideFromDirection)GetValue(SlideFromDirectionProperty); }
            set { SetValue(SlideFromDirectionProperty, value); }
        }


        /// <summary>
        /// The DP for <see cref="SlideFromDirection"/>.
        /// </summary>
        public static readonly DependencyProperty SlideFromDirectionProperty =
            DependencyProperty.Register("SlideFromDirection", typeof(SlideFromDirection), typeof(ModernContentControl), new PropertyMetadata(SlideFromDirection.Right));


    }
}
