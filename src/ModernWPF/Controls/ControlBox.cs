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
        const string PART_CloseButton = "PART_CloseButton";
        const string PART_MinButton = "PART_MinButton";
        const string PART_MaxButton = "PART_MaxButton";
        const string PART_RestoreButton = "PART_RestoreButton";


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
        /// The button style dependency property.
        /// </summary>
        public static readonly DependencyProperty ButtonStyleProperty =
            DependencyProperty.Register("ButtonStyle", typeof(Style), typeof(ControlBox), new FrameworkPropertyMetadata(null));



        public Window Window
        {
            get { return (Window)GetValue(WindowProperty); }
            set { SetValue(WindowProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Window.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty WindowProperty =
            DependencyProperty.Register("Window", typeof(Window), typeof(ControlBox), new FrameworkPropertyMetadata(null, WindowChanged));
        
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
        }

        private void AttachCommand(string partName, ICommand command)
        {
            var btn = GetTemplateChild(partName) as Button;
            if (btn != null)
            {
                btn.Command = command;
            }
        }

        private ICommand _closeCommand;
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
                if (_closeCommand == null)
                {
                    _closeCommand = new RelayCommand(() =>
                    {
                        if (Window != null) { Window.Close(); }
                    });
                }
                return _closeCommand;
            }
        }

        private ICommand _maximizeCommand;
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
                if (_maximizeCommand == null)
                {
                    _maximizeCommand = new RelayCommand(() =>
                    {
                        if (Window != null) { Window.WindowState = WindowState.Maximized; }
                    }, () =>
                    {
                        return Window != null &&
                            Window.ResizeMode != ResizeMode.NoResize &&
                            Window.ResizeMode != ResizeMode.CanMinimize &&
                            Window.WindowState != WindowState.Maximized;
                    });
                }
                return _maximizeCommand;
            }
        }

        private ICommand _restoreCommand;
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
                if (_restoreCommand == null)
                {
                    _restoreCommand = new RelayCommand(() =>
                    {
                        if (Window != null) { Window.WindowState = WindowState.Normal; }
                    }, () =>
                    {
                        return Window != null &&
                            Window.ResizeMode != ResizeMode.NoResize &&
                            Window.ResizeMode != ResizeMode.CanMinimize &&
                            Window.WindowState == WindowState.Maximized;
                    });
                }
                return _restoreCommand;
            }
        }

        private ICommand _minimizeCommand;
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
                if (_minimizeCommand == null)
                {
                    _minimizeCommand = new RelayCommand(() =>
                    {
                        if (Window != null) { Window.WindowState = WindowState.Minimized; }
                    }, () =>
                    {
                        return Window != null &&
                            Window.ResizeMode != ResizeMode.NoResize;
                    });
                }
                return _minimizeCommand;
            }
        }
    }
}
