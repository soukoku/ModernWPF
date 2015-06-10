using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace ModernWPF.Messages
{
    /// <summary>
    /// Message to update UI elements bound to <see cref="ICommand"/>s.
    /// </summary>
    public class RefreshCommandsMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RefreshCommandsMessage"/> class
        /// with the current app's dispatcher.
        /// </summary>
        public RefreshCommandsMessage()
        {
            if (Application.Current != null)
            {
                Dispatcher = Application.Current.Dispatcher;
            }
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="RefreshCommandsMessage"/> class.
        /// </summary>
        /// <param name="dispatcher">The dispatcher.</param>
        public RefreshCommandsMessage(Dispatcher dispatcher)
        {
            Dispatcher = dispatcher;
        }

        /// <summary>
        /// Gets or sets the UI thread dispatcher.
        /// </summary>
        /// <value>
        /// The dispatcher.
        /// </value>
        public Dispatcher Dispatcher { get; private set; }

        /// <summary>
        /// Handles the <see cref="RefreshCommandsMessage"/>.
        /// </summary>
        public void HandleIt()
        {
            if (Dispatcher != null && !Dispatcher.CheckAccess())
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    CommandManager.InvalidateRequerySuggested();
                }));
            }
            else
            {
                CommandManager.InvalidateRequerySuggested();
            }
        }
    }
}
