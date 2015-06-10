using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

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
        public ChooseFileMessage(FileSelected callback) : this(null, null, callback) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChooseFileMessage"/> class.
        /// </summary>
        /// <param name="sender">The message's original sender.</param>
        /// <param name="callback">The callback when files are selected.</param>
        public ChooseFileMessage(object sender, FileSelected callback)
            : this(sender, null, callback) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChooseFileMessage"/> class.
        /// </summary>
        /// <param name="sender">The message's original sender.</param>
        /// <param name="target">The message's intended target.</param>
        /// <param name="callback">The callback when files are selected.</param>
        public ChooseFileMessage(object sender, object target, FileSelected callback)
            : base(sender, target)
        {
            _callback = callback;
        }

        FileSelected _callback;

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


        /// <summary>
        /// Handles the <see cref="ChooseFileMessage" /> on a window by showing a <see cref="FileDialog" /> based on the message options.
        /// </summary>
        /// <param name="owner">The owner.</param>
        public virtual void HandleWithPlatform(Window owner)
        {
            FileDialog dialog = null;

            switch (Purpose)
            {
                case FilePurpose.OpenMultiple:
                    var d = new OpenFileDialog();
                    d.Multiselect = true;
                    dialog = d;
                    break;
                case FilePurpose.OpenSingle:
                    dialog = new OpenFileDialog();
                    break;
                case FilePurpose.Save:
                    dialog = new SaveFileDialog();
                    break;
            }

            if (dialog != null)
            {
                dialog.Title = Caption;

                if (!string.IsNullOrEmpty(InitialFolder))
                    dialog.InitialDirectory = InitialFolder;
                if (!string.IsNullOrEmpty(InitialFileName))
                    dialog.FileName = InitialFileName;
                if (!string.IsNullOrEmpty(Filters))
                    dialog.Filter = Filters;

                var result = owner == null ? dialog.ShowDialog().GetValueOrDefault() : dialog.ShowDialog(owner).GetValueOrDefault();
                if (result)
                {
                    if (owner == null || owner.CheckAccess())
                    {
                        DoCallback(dialog.FileNames);
                    }
                    else
                    {
                        owner.Dispatcher.BeginInvoke(new Action(() =>
                        {
                            DoCallback(dialog.FileNames);
                        }));
                    }
                }
            }
        }

    }

    /// <summary>
    /// Delegate for file selected callback.
    /// </summary>
    /// <param name="filePaths">The file paths.</param>
    public delegate void FileSelected(IEnumerable<string> filePaths);
}
