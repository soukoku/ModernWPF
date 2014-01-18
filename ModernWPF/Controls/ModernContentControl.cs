using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace ModernWPF.Controls
{
    // http://xamlcoder.com/blog/2010/11/04/creating-a-metro-ui-style-control/

    /// <summary>
    /// A <see cref="ContentControl"/> that animates content in.
    /// </summary>
    [TemplatePart(Name = PARTContent, Type = typeof(ContentPresenter))]
    public class ModernContentControl : ContentControl
    {
        static ModernContentControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ModernContentControl), new FrameworkPropertyMetadata(typeof(ModernContentControl)));
        }

        const string PARTContent = "PART_Content";

        ContentPresenter _presenter;

        /// <summary>
        /// Initializes a new instance of the <see cref="ModernContentControl"/> class.
        /// </summary>
        public ModernContentControl()
        {
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                this.Loaded += (s, e) =>
                {
                    VisualStateManager.GoToState(this, "AfterLoaded", Animation.ShouldAnimate);
                    if (Animation.ShouldAnimate)
                    {
                        Animation.SlideIn(_presenter, TimeSpan.FromMilliseconds(500));
                    }
                };
                //this.DataContextChanged += (s, e) =>
                //{
                //    VisualStateManager.GoToState(this, "AfterLoaded", Animation.ShouldAnimate);
                //};
                this.Unloaded += (s, e) =>
                {
                    VisualStateManager.GoToState(this, "AfterUnloaded", false);
                };
            }
        }

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes call <see cref="M:System.Windows.FrameworkElement.ApplyTemplate" />.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _presenter = GetTemplateChild(PARTContent) as ContentPresenter;
        }
    }
}
