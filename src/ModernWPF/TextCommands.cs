using ModernWPF.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Input;

namespace ModernWPF
{
    /// <summary>
    /// Contains commands for text box types.
    /// </summary>
    public static class TextCommands
    {
        private static ICommand _clearTextBoxCommand;
        /// <summary>
        /// Gets the command that clears a <see cref="TextBox"/>.
        /// </summary>
        /// <value>
        /// The clear text command.
        /// </value>
        public static ICommand ClearTextBoxCommand
        {
            get
            {
                if (_clearTextBoxCommand == null)
                {
                    _clearTextBoxCommand = new RelayCommand<TextBox>(box =>
                    {
                        if (box != null)
                        {
                            box.Clear();
                            box.Focus();
                        }
                    }, box =>
                    {
                        return box != null && !box.IsReadOnly && !string.IsNullOrEmpty(box.Text);
                    });
                }
                return _clearTextBoxCommand;
            }
        }

        private static ICommand _clearPasswordBoxCommand;
        /// <summary>
        /// Gets the command that clears a <see cref="PasswordBox"/>.
        /// </summary>
        /// <value>
        /// The clear text command.
        /// </value>
        public static ICommand ClearPasswordBoxCommand
        {
            get
            {
                if (_clearPasswordBoxCommand == null)
                {
                    _clearPasswordBoxCommand = new RelayCommand<PasswordBox>(box =>
                    {
                        if (box != null)
                        {
                            box.Clear();
                            box.Focus();
                        }
                    }, box =>
                    {
                        return box != null && box.SecurePassword.Length > 0;
                    });
                }
                return _clearPasswordBoxCommand;
            }
        }
    }
}
