using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernWPF.Messages
{
    /// <summary>
    /// Message to open Windows Explorer with optional folder to open OR item to select.
    /// </summary>
    public class OpenExplorerMessage : MessageBase
    {
        /// <summary>
        /// Gets or sets the folder path to open initially.
        /// This is exclusive with <see cref="SelectedPath"/>.
        /// </summary>
        /// <value>
        /// The folder path.
        /// </value>
        public string FolderPath { get; set; }

        /// <summary>
        /// Gets or sets the path to show as selected initially.
        /// This is exclusive with <see cref="FolderPath"/>.
        /// </summary>
        /// <value>
        /// The selected path.
        /// </value>
        public string SelectedPath { get; set; }



        /// <summary>
        /// Handles the <see cref="ChooseFileMessage"/>.
        /// </summary>
        public void HandleWithPlatform()
        {
            if (!string.IsNullOrWhiteSpace(SelectedPath))
            {
                using (Process.Start("explorer", string.Format("/select,{0}", SelectedPath))) { }
            }
            else if (!string.IsNullOrWhiteSpace(FolderPath))
            {
                using (Process.Start("explorer", FolderPath)) { }
            }
            else
            {
                using (Process.Start("explorer")) { }
            }
        }

    }
}
