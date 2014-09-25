using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernWPF.Messages
{
    /// <summary>
    /// Message for choosing file(s) for opening/saving.
    /// </summary>
    public class ChooseFileMessage : MessageBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChooseFileMessage"/> class.
        /// </summary>
        /// <param name="callback">The callback when files are selected.</param>
        public ChooseFileMessage(Action<IEnumerable<string>> callback) : this(null, null, callback) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChooseFileMessage"/> class.
        /// </summary>
        /// <param name="sender">The message's original sender.</param>
        /// <param name="callback">The callback when files are selected.</param>
        public ChooseFileMessage(object sender, Action<IEnumerable<string>> callback)
            : this(sender, null, callback) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChooseFileMessage"/> class.
        /// </summary>
        /// <param name="sender">The message's original sender.</param>
        /// <param name="target">The message's intended target.</param>
        /// <param name="callback">The callback when files are selected.</param>
        public ChooseFileMessage(object sender, object target, Action<IEnumerable<string>> callback)
            : base(sender, target)
        {
            _callback = callback;
        }

        Action<IEnumerable<string>> _callback;

        /// <summary>
        /// Gets or sets the UI caption.
        /// </summary>
        /// <value>
        /// The caption.
        /// </value>
        public string Caption { get; set; }

        /// <summary>
        /// Gets or sets the initial name of the file.
        /// </summary>
        /// <value>
        /// The initial name of the file.
        /// </value>
        public string InitialFileName { get; set; }

        /// <summary>
        /// Gets or sets the initial folder.
        /// </summary>
        /// <value>
        /// The initial folder.
        /// </value>
        public string InitialFolder { get; set; }

        /// <summary>
        /// Gets or sets the file type filters.
        /// </summary>
        /// <value>
        /// The filters.
        /// </value>
        public string Filters { get; set; }

        /// <summary>
        /// Gets or sets the file selection purpose.
        /// </summary>
        /// <value>
        /// The purpose.
        /// </value>
        public FilePurpose Purpose { get; set; }


        /// <summary>
        /// Indicates the purpose for a <see cref="ChooseFileMessage"/>.
        /// </summary>
        public enum FilePurpose
        {
            /// <summary>
            /// Choose a single file to open.
            /// </summary>
            OpenSingle,
            /// <summary>
            /// Choose multiple files to open.
            /// </summary>
            OpenMultiple,
            /// <summary>
            /// Choose a single file to save.
            /// </summary>
            Save
        }

        /// <summary>
        /// Does the callback to notify sender of selected files.
        /// </summary>
        /// <param name="files">The files.</param>
        public void DoCallback(params string[] files)
        {
            if (_callback != null)
            {
                _callback(files);
            }
        }
    }
}
