using CommonWin32;
using CommonWin32.API;
using CommonWin32.Monitors;
using CommonWin32.Rectangles;
using CommonWin32.Windows;
using ModernWPF.Controls;
using ModernWPF.Converters;
using ModernWPF.Internal;
using ModernWPF.Native;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;

namespace ModernWPF
{
    // yes this is the same idea as the WindowChrome class in framework 4.5 but greatly improved for modern style.

    /// <summary>
    /// Attached property class for making a <see cref="Window"/> modern.
    /// </summary>
    public class Chrome : Freezable
    {
        #region DPs

        #region hit test attached dp

        /// <summary>
        /// Attached property to mark a UI element as hit-testable when in the window caption area.
        /// </summary>
        public static readonly DependencyProperty IsHitTestVisibleProperty =
            DependencyProperty.RegisterAttached("IsHitTestVisible", typeof(bool), typeof(Chrome),
            new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.Inherits));

        /// <summary>
        /// Gets the IsHitTestVisible property for the element.
        /// </summary>
        /// <param name="inputElement">The input element.</param>
        /// <returns></returns>
        public static bool GetIsHitTestVisible(IInputElement inputElement)
        {
            DependencyObject obj2 = inputElement as DependencyObject;
            if (obj2 == null)
            {
                return false;
            }
            return (bool)obj2.GetValue(IsHitTestVisibleProperty);
        }

        /// <summary>
        /// Sets the IsHitTestVisible property for the element.
        /// </summary>
        /// <param name="inputElement">The input element.</param>
        /// <param name="hitTestVisible">if set to <c>true</c> then the element is hit test visible in chrome.</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="System.ArgumentException"></exception>
        public static void SetIsHitTestVisible(IInputElement inputElement, bool hitTestVisible)
        {
            if (inputElement == null) { throw new ArgumentNullException("inputElement"); }

            DependencyObject obj2 = inputElement as DependencyObject;
            if (obj2 == null)
            {
                throw new ArgumentException("The element must be a DependencyObject", "inputElement");
            }
            obj2.SetValue(IsHitTestVisibleProperty, hitTestVisible);
        }




        /// <summary>
        /// Gets the IsCaption value.
        /// </summary>
        /// <param name="inputElement">The input element.</param>
        /// <returns></returns>
        public static bool GetIsCaption(IInputElement inputElement)
        {
            DependencyObject obj2 = inputElement as DependencyObject;
            if (obj2 == null)
            {
                return false;
            }
            return (bool)obj2.GetValue(IsCaptionProperty);
        }
        /// <summary>
        /// Sets the IsCaption value.
        /// </summary>
        /// <param name="inputElement">The input element.</param>
        /// <param name="isCaption">if set to <c>true</c> then the element is hit test visible as caption.</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="System.ArgumentException"></exception>
        public static void SetIsCaption(IInputElement inputElement, bool isCaption)
        {
            if (inputElement == null) { throw new ArgumentNullException("inputElement"); }

            DependencyObject obj2 = inputElement as DependencyObject;
            if (obj2 == null)
            {
                throw new ArgumentException("The element must be a DependencyObject", "inputElement");
            }
            obj2.SetValue(IsCaptionProperty, isCaption);
        }

        /// <summary>
        /// Attached property to mark a UI element as caption during hit-tests.
        /// </summary>
        public static readonly DependencyProperty IsCaptionProperty =
            DependencyProperty.RegisterAttached("IsCaption", typeof(bool), typeof(Chrome),
            new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.Inherits));



        #endregion

        #region chrome attached dp

        /// <summary>
        /// Gets the chrome.
        /// </summary>
        /// <param name="window">The window.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">window</exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
        public static Chrome GetChrome(Window window)
        {
            if (window == null) { throw new ArgumentNullException("window"); }
            return (Chrome)window.GetValue(Chrome.ChromeProperty);
        }

        /// <summary>
        /// Sets the chrome.
        /// </summary>
        /// <param name="window">The window.</param>
        /// <param name="chrome">The chrome.</param>
        /// <exception cref="System.ArgumentNullException">window</exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
        public static void SetChrome(Window window, Chrome chrome)
        {
            if (window == null) { throw new ArgumentNullException("window"); }
            window.SetValue(Chrome.ChromeProperty, chrome);
        }

        static bool __legacyBorder = false;


        /// <summary>
        /// The modern chrome attached property.
        /// </summary>
        public static readonly DependencyProperty ChromeProperty =
            DependencyProperty.RegisterAttached("Chrome", typeof(Chrome), typeof(Chrome), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits, ChromeChanged));

        private static void ChromeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (DesignerProperties.GetIsInDesignMode(d)) { return; }

            Window window = d as Window;
            if (window != null)
            {
                // don't care about old chrome since it has no state
                Chrome newChrome = e.NewValue as Chrome;

                if (__legacyBorder)
                {
                    if (newChrome == null)
                    {
                        LegacyBorderManager.SetManager(window, null);
                    }
                    else if (e.NewValue != e.OldValue)
                    {
                        var worker = LegacyBorderManager.GetManager(window);
                        if (worker == null)
                        {
                            worker = new LegacyBorderManager();
                            LegacyBorderManager.SetManager(window, worker);
                        }
                        else
                        {
                            worker.UpdateChrome(newChrome);
                        }
                    }
                }
                else
                {
                    if (newChrome == null)
                    {
                        BorderManager.SetManager(window, null);
                    }
                    else if (e.NewValue != e.OldValue)
                    {
                        var worker = BorderManager.GetManager(window);
                        if (worker == null)
                        {
                            worker = new BorderManager();
                            BorderManager.SetManager(window, worker);
                        }
                        else
                        {
                            worker.UpdateChrome(newChrome);
                        }
                    }
                }
            }
        }

        #endregion

        #region in window appearance attached dp

        /// <summary>
        /// Gets the height of the window caption area.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns></returns>
        public static double GetCaptionHeight(DependencyObject obj)
        {
            if (obj == null) { throw new ArgumentNullException("obj"); }
            return (double)obj.GetValue(CaptionHeightProperty);
        }

        /// <summary>
        /// Sets the height of the window caption area.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="value">The value.</param>
        public static void SetCaptionHeight(DependencyObject obj, double value)
        {
            if (obj == null) { throw new ArgumentNullException("obj"); }
            obj.SetValue(CaptionHeightProperty, value);
        }

        /// <summary>
        /// The dependency property for CaptionHeight.
        /// </summary>
        public static readonly DependencyProperty CaptionHeightProperty =
            DependencyProperty.RegisterAttached("CaptionHeight", typeof(double), typeof(Chrome), new UIPropertyMetadata(-1d)); //new PropertyMetadata(32d));


        /// <summary>
        /// Gets the value indicating whether to show caption text on the window.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns></returns>
        public static bool GetShowCaptionText(DependencyObject obj)
        {
            if (obj == null) { throw new ArgumentNullException("obj"); }
            return (bool)obj.GetValue(ShowCaptionTextProperty);
        }

        /// <summary>
        /// Gets the value indicating whether to show caption text on the window.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="value">if set to <c>true</c> then show window title.</param>
        public static void SetShowCaptionText(DependencyObject obj, bool value)
        {
            if (obj == null) { throw new ArgumentNullException("obj"); }
            obj.SetValue(ShowCaptionTextProperty, value);
        }

        /// <summary>
        /// The dependency property for ShowCaptionText.
        /// </summary>
        public static readonly DependencyProperty ShowCaptionTextProperty =
            DependencyProperty.RegisterAttached("ShowCaptionText", typeof(bool), typeof(Chrome), new PropertyMetadata(true));


        /// <summary>
        /// Gets the value indicating whether to show caption icon on the window.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns></returns>
        public static bool GetShowCaptionIcon(DependencyObject obj)
        {
            if (obj == null) { throw new ArgumentNullException("obj"); }
            return (bool)obj.GetValue(ShowCaptionIconProperty);
        }

        /// <summary>
        /// Sets the value indicating whether to show caption text on the window.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="value">if set to <c>true</c> [value].</param>
        public static void SetShowCaptionIcon(DependencyObject obj, bool value)
        {
            if (obj == null) { throw new ArgumentNullException("obj"); }
            obj.SetValue(ShowCaptionIconProperty, value);
        }

        /// <summary>
        /// The dependency property for ShowCaptionIcon.
        /// </summary>
        public static readonly DependencyProperty ShowCaptionIconProperty =
            DependencyProperty.RegisterAttached("ShowCaptionIcon", typeof(bool), typeof(Chrome), new PropertyMetadata(true));



        /// <summary>
        /// Gets whether to show control boxes in ModernWindow style.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns></returns>
        public static bool GetShowControlBoxes(DependencyObject obj)
        {
            if (obj == null) { throw new ArgumentNullException("obj"); }
            return (bool)obj.GetValue(ShowControlBoxesProperty);
        }

        /// <summary>
        /// Sets whether to show control boxes.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="value">if set to <c>true</c> then show the control boxes.</param>
        public static void SetShowControlBoxes(DependencyObject obj, bool value)
        {
            if (obj == null) { throw new ArgumentNullException("obj"); }
            obj.SetValue(ShowControlBoxesProperty, value);
        }

        /// <summary>
        /// The dependency property for ShowControlBoxes.
        /// </summary>
        public static readonly DependencyProperty ShowControlBoxesProperty =
            DependencyProperty.RegisterAttached("ShowControlBoxes", typeof(bool), typeof(Chrome), new PropertyMetadata(true));



        #endregion

        #region chrome border dp

        /// <summary>
        /// Gets the resize border thickness.
        /// </summary>
        /// <value>
        /// The resize border thickness.
        /// </value>
        public Thickness ResizeBorderThickness
        {
            get { return (Thickness)GetValue(ResizeBorderThicknessProperty); }
        }

        /// <summary>
        /// The dependency property for <see cref="ResizeBorderThickness"/>.
        /// </summary>
        public static readonly DependencyProperty ResizeBorderThicknessProperty =
            DependencyProperty.Register("ResizeBorderThickness", typeof(Thickness), typeof(Chrome), new PropertyMetadata(new Thickness(8)));


        /// <summary>
        /// Gets or sets the active border brush.
        /// </summary>
        /// <value>
        /// The active border brush.
        /// </value>
        [Category("Appearance")]
        public Brush ActiveBorderBrush
        {
            get { return (Brush)GetValue(ActiveBorderBrushProperty); }
            set { SetValue(ActiveBorderBrushProperty, value); }
        }

        /// <summary>
        /// The dependency property for <see cref="ActiveBorderBrush"/>.
        /// </summary>
        public static readonly DependencyProperty ActiveBorderBrushProperty =
            DependencyProperty.Register("ActiveBorderBrush", typeof(Brush), typeof(Chrome), new PropertyMetadata(Brushes.DimGray));


        /// <summary>
        /// Gets or sets the inactive border brush.
        /// </summary>
        /// <value>
        /// The inactive border brush.
        /// </value>
        [Category("Appearance")]
        public Brush InactiveBorderBrush
        {
            get { return (Brush)GetValue(InactiveBorderBrushProperty); }
            set { SetValue(InactiveBorderBrushProperty, value); }
        }

        /// <summary>
        /// The dependency property for <see cref="InactiveBorderBrush"/>.
        /// </summary>
        public static readonly DependencyProperty InactiveBorderBrushProperty =
            DependencyProperty.Register("InactiveBorderBrush", typeof(Brush), typeof(Chrome), new PropertyMetadata(Brushes.LightGray));


        /// <summary>
        /// Gets or sets the active caption brush.
        /// </summary>
        /// <value>
        /// The active caption brush.
        /// </value>
        [Category("Appearance")]
        public Brush ActiveCaptionBrush
        {
            get { return (Brush)GetValue(ActiveCaptionBrushProperty); }
            set { SetValue(ActiveCaptionBrushProperty, value); }
        }

        /// <summary>
        /// The dependency property for <see cref="ActiveCaptionBrush"/>.
        /// </summary>
        public static readonly DependencyProperty ActiveCaptionBrushProperty =
            DependencyProperty.Register("ActiveCaptionBrush", typeof(Brush), typeof(Chrome), new PropertyMetadata(Brushes.Transparent));


        /// <summary>
        /// Gets or sets the inactive caption brush.
        /// </summary>
        /// <value>
        /// The inactive caption brush.
        /// </value>
        [Category("Appearance")]
        public Brush InactiveCaptionBrush
        {
            get { return (Brush)GetValue(InactiveCaptionBrushProperty); }
            set { SetValue(InactiveCaptionBrushProperty, value); }
        }

        /// <summary>
        /// The dependency property for <see cref="InactiveCaptionBrush"/>.
        /// </summary>
        public static readonly DependencyProperty InactiveCaptionBrushProperty =
            DependencyProperty.Register("InactiveCaptionBrush", typeof(Brush), typeof(Chrome), new PropertyMetadata(Brushes.Transparent));


        /// <summary>
        /// Gets or sets the active caption foreground.
        /// </summary>
        /// <value>
        /// The active caption foreground.
        /// </value>
        [Category("Appearance")]
        public Brush ActiveCaptionForeground
        {
            get { return (Brush)GetValue(ActiveCaptionForegroundProperty); }
            set { SetValue(ActiveCaptionForegroundProperty, value); }
        }

        /// <summary>
        /// The dependency property for <see cref="ActiveCaptionForeground"/>.
        /// </summary>
        public static readonly DependencyProperty ActiveCaptionForegroundProperty =
            DependencyProperty.Register("ActiveCaptionForeground", typeof(Brush), typeof(Chrome), new PropertyMetadata(Brushes.Black));


        /// <summary>
        /// Gets or sets the inactive caption foreground.
        /// </summary>
        /// <value>
        /// The inactive caption foreground.
        /// </value>
        [Category("Appearance")]
        public Brush InactiveCaptionForeground
        {
            get { return (Brush)GetValue(InactiveCaptionForegroundProperty); }
            set { SetValue(InactiveCaptionForegroundProperty, value); }
        }

        /// <summary>
        /// The dependency property for <see cref="InactiveCaptionForeground"/>.
        /// </summary>
        public static readonly DependencyProperty InactiveCaptionForegroundProperty =
            DependencyProperty.Register("InactiveCaptionForeground", typeof(Brush), typeof(Chrome), new PropertyMetadata(Brushes.DimGray));

        #endregion


        /// <summary>
        /// Gets the extra content on the caption area.
        /// </summary>
        /// <param name="window">The object.</param>
        /// <returns></returns>
        public static object GetCaptionExtraContent(Window window)
        {
            if (window == null) { throw new ArgumentNullException("window"); }
            return (object)window.GetValue(CaptionExtraContentProperty);
        }

        /// <summary>
        /// Sets the extra content on the caption area.
        /// </summary>
        /// <param name="window">The object.</param>
        /// <param name="value">The value.</param>
        public static void SetCaptionExtraContent(Window window, object value)
        {
            if (window == null) { throw new ArgumentNullException("window"); }
            window.SetValue(CaptionExtraContentProperty, value);
        }

        /// <summary>
        /// The dependency property for <see cref="CaptionExtraContent"/>.
        /// </summary>
        public static readonly DependencyProperty CaptionExtraContentProperty =
            DependencyProperty.RegisterAttached("CaptionExtraContent", typeof(object), typeof(Chrome), new PropertyMetadata(null));




        #endregion

        /// <summary>
        /// When implemented in a derived class, creates a new instance of the <see cref="T:System.Windows.Freezable" /> derived class.
        /// </summary>
        /// <returns>
        /// The new instance.
        /// </returns>
        protected override Freezable CreateInstanceCore()
        {
            return new Chrome();
        }
    }
}
