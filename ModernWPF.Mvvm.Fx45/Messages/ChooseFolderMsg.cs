using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernWPF.Messages
{
    /// <summary>
    /// Message for choosing a folder.
    /// </summary>
    public class ChooseFolderMsg : MessageBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChooseFolderMsg"/> class.
        /// </summary>
        /// <param name="callback">The callback when a folder is chosen.</param>
        public ChooseFolderMsg(Action<string> callback)
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
    }
}
