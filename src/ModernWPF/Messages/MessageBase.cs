using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ModernWPF.Messages
{
    /// <summary>
    /// Base class for messages.
    /// </summary>
    public abstract class MessageBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MessageBase"/> class.
        /// </summary>
        protected MessageBase() : this(null, null) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="MessageBase"/> class.
        /// </summary>
        /// <param name="sender">The sender.</param>
        protected MessageBase(object sender) : this(sender, null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageBase"/> class.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="target">The target.</param>
        protected MessageBase(object sender, object target)
        {
            Sender = sender;
            Target = target;
        }

        /// <summary>
        /// Gets or sets the sender.
        /// </summary>
        /// <value>
        /// The sender.
        /// </value>
        public object Sender { get; protected set; }

        /// <summary>
        /// Gets or sets the target.
        /// </summary>
        /// <value>
        /// The target.
        /// </value>
        public object Target { get; protected set; }
    }
}
