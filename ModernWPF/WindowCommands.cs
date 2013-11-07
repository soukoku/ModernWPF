using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;

namespace ModernWPF
{
    /// <summary>
    /// Provides a set of window control commands
    /// as a bindable resource object.
    /// </summary>
    public class WindowCommands
    {
        bool _hooked = false;

        void TryHook(Window window)
        {
            if (_hooked || window == null) { return; }
            window.StateChanged += (s, e) => { CommandManager.InvalidateRequerySuggested(); };
            _hooked = true;
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
                    _closeCommand = new RelayCommand<Window>(window =>
                    {
                        if (window != null) { window.Close(); }
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
                    _maximizeCommand = new RelayCommand<Window>(window =>
                    {
                        if (window != null) { window.WindowState = WindowState.Maximized; }
                    }, window =>
                    {
                        TryHook(window);
                        return window != null &&
                            window.ResizeMode != ResizeMode.NoResize &&
                            window.ResizeMode != ResizeMode.CanMinimize &&
                            window.WindowState != WindowState.Maximized;
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
                    _restoreCommand = new RelayCommand<Window>(window =>
                    {
                        if (window != null) { window.WindowState = WindowState.Normal; }
                    }, window =>
                    {
                        TryHook(window);
                        return window != null &&
                            window.ResizeMode != ResizeMode.NoResize &&
                            window.ResizeMode != ResizeMode.CanMinimize &&
                            window.WindowState == WindowState.Maximized;
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
                    _minimizeCommand = new RelayCommand<Window>(window =>
                    {
                        if (window != null) { window.WindowState = WindowState.Minimized; }
                    }, window =>
                    {
                        return window != null &&
                            window.ResizeMode != ResizeMode.NoResize;
                    });
                }
                return _minimizeCommand;
            }
        }
    }
}
