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
        public MessageBoxMessage(string content, Action<MessageBoxResult> callback) : this(null, null, content, callback) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageBoxMessage" /> class.
        /// </summary>
        /// <param name="sender">The message's original sender.</param>
        /// <param name="content">The content.</param>
        public MessageBoxMessage(object sender, string content, Action<MessageBoxResult> callback) : this(sender, null, content, callback) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageBoxMessage" /> class.
        /// </summary>
        /// <param name="sender">The message's original sender.</param>
        /// <param name="target">The message's intended target.</param>
        /// <param name="content">The content.</param>
        public MessageBoxMessage(object sender, object target, string content, Action<MessageBoxResult> callback)
            : base(sender, target)
        {
            Content = content;
            Callback = callback;
        }

        /// <summary>
        /// Gets or sets the available buttons.
        /// </summary>
        /// <value>
        /// The button.
        /// </value>
        public MessageBoxButton Button { get; set; }

        /// <summary>
        /// Gets or sets the caption.
        /// </summary>
        /// <value>
        /// The caption.
        /// </value>
        public string Caption { get; set; }

        /// <summary>
        /// Gets the content.
        /// </summary>
        /// <value>
        /// The content.
        /// </value>
        public string Content { get; private set; }

        /// <summary>
        /// Gets or sets the default result.
        /// </summary>
        /// <value>
        /// The default result.
        /// </value>
        public MessageBoxResult DefaultResult { get; set; }

        /// <summary>
        /// Gets or sets the icon.
        /// </summary>
        /// <value>
        /// The icon.
        /// </value>
        public MessageBoxImage Icon { get; set; }

        /// <summary>
        /// Gets or sets the options.
        /// </summary>
        /// <value>
        /// The options.
        /// </value>
        public MessageBoxOptions Options { get; set; }

        /// <summary>
        /// Gets the callback.
        /// </summary>
        public Action<MessageBoxResult> Callback { get; private set; }


        /// <summary>
        /// Handles a basic <see cref="MessageBoxMessage" /> on a window by showing a <see cref="ModernMessageBox" />
        /// and invokes the callback.
        /// </summary>
        /// <param name="owner">The owner.</param>
        public void HandleWithModern(Window owner)
        {
            if (owner == null) { throw new ArgumentNullException("owner"); }

            var res = ModernMessageBox.Show(owner, Content, Caption, Button, Icon, DefaultResult);
            if (Callback != null)
            {
                if (owner == null || owner.CheckAccess())
                {
                    Callback(res);
                }
                else
                {
                    owner.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        Callback(res);
                    }));
                }
            }
        }

        /// <summary>
        /// Handles a basic <see cref="MessageBoxMessage" /> on a window by showing built-in <see cref="MessageBox"/>
        /// and invokes the callback.
        /// </summary>
        /// <param name="owner">The owner.</param>
        public void HandleWithPlatform(Window owner)
        {
            MessageBoxResult res = MessageBoxResult.None;
            if (owner == null)
            {
                res = MessageBox.Show(Content, Caption, Button, Icon, DefaultResult, Options);
            }
            else
            {
                res = MessageBox.Show(owner, Content, Caption, Button, Icon, DefaultResult, Options);
            }

            if (Callback != null)
            {
                if (owner == null || owner.CheckAccess())
                {
                    Callback(res);
                }
                else
                {
                    owner.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        Callback(res);
                    }));
                }
            }
        }

    }
}
