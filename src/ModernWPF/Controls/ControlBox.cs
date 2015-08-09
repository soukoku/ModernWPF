using ModernWPF;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Markup;

namespace ModernWPF.Controls
{
    /// <summary>
    /// A UI piece for window control box (min/max/restore buttons).
    /// </summary>
    [TemplatePart(Name = PART_CloseButton, Type = typeof(ControlBox))]
    [TemplatePart(Name = PART_MinButton, Type = typeof(ControlBox))]
    [TemplatePart(Name = PART_MaxButton, Type = typeof(ControlBox))]
    [TemplatePart(Name = PART_RestoreButton, Type = typeof(ControlBox))]
    public class ControlBox : Control
    {
        /// <summary>
        /// Name of the close button in template.
        /// </summary>
        protected const string PART_CloseButton = "PART_CloseButton";
        /// <summary>
        /// Name of the minimize button in template.
        /// </summary>
        protected const string PART_MinButton = "PART_MinButton";
        /// <summary>
        /// Name of the maximize button in template.
        /// </summary>
        protected const string PART_MaxButton = "PART_MaxButton";
        /// <summary>
        /// Name of the restore button in template.
        /// </summary>
        protected const string PART_RestoreButton = "PART_RestoreButton";


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline")]
        static ControlBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ControlBox), new FrameworkPropertyMetadata(typeof(ControlBox)));
            IsTabStopProperty.OverrideMetadata(typeof(ControlBox), new FrameworkPropertyMetadata(false));
        }


        /// <summary>
        /// Gets or sets the button style.
        /// </summary>
        /// <value>
        /// The button style.
        /// </value>
        public Style ButtonStyle
        {
            get { return (Style)GetValue(ButtonStyleProperty); }
            set { SetValue(ButtonStyleProperty, value); }
        }

        /// <summary>
        /// The <see cref="ButtonStyle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ButtonStyleProperty =
            DependencyProperty.Register("ButtonStyle", typeof(Style), typeof(ControlBox), new FrameworkPropertyMetadata(null));



        /// <summary>
        /// Gets or sets the associated <see cref="Window"/> to be controlled by this control box.
        /// </summary>
        /// <value>
        /// The window.
        /// </value>
        [TypeConverter(typeof(NameReferenceConverter))]
        public Window TargetWindow
        {
            get { return (Window)GetValue(WindowProperty); }
            set { SetValue(WindowProperty, value); }
        }


        /// <summary>
        /// The <see cref="TargetWindow"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty WindowProperty =
            DependencyProperty.Register("TargetWindow", typeof(Window), typeof(ControlBox), new FrameworkPropertyMetadata(null, WindowChanged));

        static void WindowChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var old = e.OldValue as Window;
            if (old != null)
            {
                old.StateChanged -= WindowStateChanged;
            }

            var newWin = e.NewValue as Window;
            if (newWin != null)
            {
                newWin.StateChanged += WindowStateChanged;
            }
        }

        private static void WindowStateChanged(object sender, EventArgs e)
        {
            CommandManager.InvalidateRequerySuggested();
        }


        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            if (DesignerProperties.GetIsInDesignMode(this)) { return; }

            AttachCommand(PART_CloseButton, CloseCommand);
            AttachCommand(PART_MinButton, MinimizeCommand);
            AttachCommand(PART_RestoreButton, RestoreCommand);
            AttachCommand(PART_MaxButton, MaximizeCommand);

            if (TargetWindow == null)
            {
                TargetWindow = this.FindParentInVisualTree<Window>();
            }
        }

        private void AttachCommand(string partName, ICommand command)
        {
            var btn = GetTemplateChild(partName) as Button;
            if (btn != null)
            {
                btn.Command = command;
            }
        }

        private RelayCommand _closeCommand;
        /// <summary>
        /// Gets the command that closes the window.
        /// </summary>
        /// <value>
        /// The close command.
        /// </value>
        public ICommand CloseCommand
        {
            get
            {
                return _closeCommand ?? (
                    _closeCommand = new RelayCommand(() =>
                    {
                        if (TargetWindow != null) { TargetWindow.Close(); }
                    }, () => TargetWindow != null)
                );
            }
        }

        private RelayCommand _maximizeCommand;
        /// <summary>
        /// Gets the command that maximizes the window.
        /// </summary>
        /// <value>
        /// The maximize command.
        /// </value>
        public ICommand MaximizeCommand
        {
            get
            {
                return _maximizeCommand ?? (
                    _maximizeCommand = new RelayCommand(() =>
                    {
                        if (TargetWindow != null) { TargetWindow.WindowState = WindowState.Maximized; }
                    }, () =>
                    {
                        return TargetWindow != null &&
                            TargetWindow.ResizeMode != ResizeMode.NoResize &&
                            TargetWindow.ResizeMode != ResizeMode.CanMinimize &&
                            TargetWindow.WindowState != WindowState.Maximized;
                    })
                );
            }
        }

        private RelayCommand _restoreCommand;
        /// <summary>
        /// Gets the command that restores the window.
        /// </summary>
        /// <value>
        /// The restore command.
        /// </value>
        public ICommand RestoreCommand
        {
            get
            {
                return _restoreCommand ?? (
                    _restoreCommand = new RelayCommand(() =>
                    {
                        if (TargetWindow != null) { TargetWindow.WindowState = WindowState.Normal; }
                    }, () =>
                    {
                        return TargetWindow != null &&
                            TargetWindow.ResizeMode != ResizeMode.NoResize &&
                            TargetWindow.ResizeMode != ResizeMode.CanMinimize &&
                            TargetWindow.WindowState == WindowState.Maximized;
                    })
                );
            }
        }

        private RelayCommand _minimizeCommand;
        /// <summary>
        /// Gets the command that minimizes the window.
        /// </summary>
        /// <value>
        /// The minimize command.
        /// </value>
        public ICommand MinimizeCommand
        {
            get
            {
                return _minimizeCommand ?? (
                    _minimizeCommand = new RelayCommand(() =>
                    {
                        if (TargetWindow != null) { TargetWindow.WindowState = WindowState.Minimized; }
                    }, () =>
                    {
                        return TargetWindow != null &&
                            TargetWindow.ResizeMode != ResizeMode.NoResize;
                    })
                );
            }
        }
    }
}
