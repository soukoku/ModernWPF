using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Input;

namespace ModernWPF
{
    class TextCommands
    {
        private ICommand _clearTextCommand;
        /// <summary>
        /// Gets the command that clears a TextBox.
        /// </summary>
        /// <value>
        /// The clear text command.
        /// </value>
        public ICommand ClearTextCommand
        {
            get
            {
                if (_clearTextCommand == null)
                {
                    _clearTextCommand = new RelayCommand<TextBox>(box =>
                    {
                        if (box != null) { box.Clear(); }
                    }, box =>
                    {
                        return box != null && !box.IsReadOnly && !string.IsNullOrEmpty(box.Text);
                    });
                }
                return _clearTextCommand;
            }
        }
    }
}
