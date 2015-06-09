using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ModernWPF.Messages
{
    /// <summary>
    /// Message for notifying the app is closing.
    /// </summary>
    public class AppClosingMessage : MessageBase
    {
        /// <summary>
        /// Gets or sets a value indicating whether the closing should be canceled.
        /// </summary>
        /// <value>
        ///   <c>true</c> to cancel; otherwise, <c>false</c>.
        /// </value>
        public bool Cancel { get; set; }
    }
}
