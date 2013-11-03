using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace ModernWPF.Controls
{
    // http://xamlcoder.com/blog/2010/11/04/creating-a-metro-ui-style-control/

    /// <summary>
    /// A <see cref="ContentControl"/> that animates content in.
    /// </summary>
    public class ModernContentControl : ContentControl
    {
        static ModernContentControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ModernContentControl), new FrameworkPropertyMetadata(typeof(ModernContentControl)));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ModernContentControl"/> class.
        /// </summary>
        public ModernContentControl()
        {
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                this.Loaded += (s, e) =>
                {
                    VisualStateManager.GoToState(this, "AfterLoaded", !SystemParameters.IsRemoteSession);
                };
                //this.DataContextChanged += (s, e) =>
                //{
                //    VisualStateManager.GoToState(this, "AfterLoaded", !SystemParameters.IsRemoteSession);
                //};
                this.Unloaded += (s, e) =>
                {
                    VisualStateManager.GoToState(this, "AfterUnloaded", false);
                };
            }
        }
    }
}
