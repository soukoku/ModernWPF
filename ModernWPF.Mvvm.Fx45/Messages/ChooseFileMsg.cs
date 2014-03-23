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
    public class ChooseFileMsg : MessageBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChooseFileMsg"/> class.
        /// </summary>
        /// <param name="callback">The callback.</param>
        public ChooseFileMsg(Action<IEnumerable<string>> callback)
        {
            _callback = callback;
        }
        Action<string[]> _callback;

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
        /// Indicates the purpose for a <see cref="ChooseFileMsg"/>.
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
