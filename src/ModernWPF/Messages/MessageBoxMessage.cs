using Microsoft.Win32;
using ModernWPF.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ModernWPF.Messages
{
    /// <summary>
    /// Message for showing typical modal dialog.
    /// </summary>
    public class MessageBoxMessage : MessageBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MessageBoxMessage" /> class.
        /// </summary>
        /// <param name="content">The content.</param>
        public MessageBoxMessage(string content) : this(null, null, content) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageBoxMessage" /> class.
        /// </summary>
        /// <param name="sender">The message's original sender.</param>
        /// <param name="content">The content.</param>
        public MessageBoxMessage(object sender, string content) : this(sender, null, content) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageBoxMessage" /> class.
        /// </summary>
        /// <param name="sender">The message's original sender.</param>
        /// <param name="target">The message's intended target.</param>
        /// <param name="content">The content.</param>
        public MessageBoxMessage(object sender, object target, string content)
            : base(sender, target)
        { Content = content; }

        public MessageBoxButton Button { get; set; }
        public string Caption { get; set; }
        public string Content { get; private set; }
        public MessageBoxResult DefaultResult { get; set; }
        public MessageBoxImage Icon { get; set; }
        public MessageBoxOptions Options { get; set; }



        /// <summary>
        /// Handles a basic <see cref="MessageBoxMessage" /> on a window by showing a <see cref="ModernMessageBox" />.
        /// </summary>
        /// <param name="owner">The owner.</param>
        /// <returns></returns>
        public MessageBoxResult HandleWithModern(Window owner)
        {
            if (owner == null) { throw new ArgumentNullException("owner"); }

            return ModernMessageBox.Show(owner, Content, Caption, Button, Icon, DefaultResult);
        }

        /// <summary>
        /// Handles a basic <see cref="MessageBoxMessage" /> on a window by showing built-in <see cref="MessageBox"/>.
        /// </summary>
        /// <param name="owner">The owner.</param>
        /// <returns></returns>
        public MessageBoxResult HandleWithPlatform(Window owner)
        {
            if (owner == null)
            {
                return MessageBox.Show(Content, Caption, Button, Icon, DefaultResult, Options);
            }
            return MessageBox.Show(owner, Content, Caption, Button, Icon, DefaultResult, Options);
        }

    }
}
