using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ModernWPF.Messages
{
    /// <summary>
    /// Message for choosing a folder.
    /// </summary>
    public class ChooseFolderMessage : MessageBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChooseFolderMessage"/> class.
        /// </summary>
        /// <param name="callback">The callback when a folder is chosen.</param>
        public ChooseFolderMessage(Action<string> callback) : this(null, null, callback) { }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="ChooseFolderMessage"/> class.
        /// </summary>
        /// <param name="sender">The message's original sender.</param>
        /// <param name="callback">The callback when a folder is chosen.</param>
        public ChooseFolderMessage(object sender, Action<string> callback)
            : this(sender, null, callback) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChooseFolderMessage"/> class.
        /// </summary>
        /// <param name="sender">The message's original sender.</param>
        /// <param name="target">The message's intended target.</param>
        /// <param name="callback">The callback when a folder is chosen.</param>
        public ChooseFolderMessage(object sender, object target, Action<string> callback)
            : base(sender, target)
        {
            _callback = callback;
        }


        Action<string> _callback;

        /// <summary>
        /// Gets or sets the UI caption.
        /// </summary>
        /// <value>
        /// The caption.
        /// </value>
        public string Caption { get; set; }

        /// <summary>
        /// Gets or sets the initial folder.
        /// </summary>
        /// <value>
        /// The initial folder.
        /// </value>
        public string InitialFolder { get; set; }

        /// <summary>
        /// Does the callback to notify sender of selected folder.
        /// </summary>
        /// <param name="folder">The folder.</param>
        public void DoCallback(string folder)
        {
            if (_callback != null)
            {
                _callback(folder);
            }
        }



        /// <summary>
        /// Handles the <see cref="ChooseFolderMessage" /> on a window by showing a folder dialog based on the message options.
        /// </summary>
        /// <param name="owner">The owner.</param>
        /// </exception>
        public virtual void HandleWithPlatform(Window owner)
        {
            using (var diag = new System.Windows.Forms.FolderBrowserDialog())
            {
                diag.ShowNewFolderButton = true;
                diag.SelectedPath = InitialFolder;

                if (diag.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    if (owner == null || owner.Dispatcher.CheckAccess())
                    {
                        DoCallback(diag.SelectedPath);
                    }
                    else
                    {
                        owner.Dispatcher.BeginInvoke(new Action(() =>
                        {
                            DoCallback(diag.SelectedPath);
                        }));
                    }
                }
            }
        }
    }
}
