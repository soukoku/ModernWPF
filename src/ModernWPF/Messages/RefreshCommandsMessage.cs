using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace ModernWPF.Messages
{
    /// <summary>
    /// Message to update UI elements bound to <see cref="ICommand"/>s.
    /// </summary>
    public class RefreshCommandsMessage
    {
        /// <summary>
        /// Handles the <see cref="RefreshCommandsMessage"/>.
        /// </summary>
        public void HandleIt()
        {
            if (Application.Current != null)
            {
                if (Application.Current.Dispatcher.CheckAccess())
                {
                    CommandManager.InvalidateRequerySuggested();
                }
                else
                {
                    Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        CommandManager.InvalidateRequerySuggested();
                    }));
                }
            }
        }
    }
}
