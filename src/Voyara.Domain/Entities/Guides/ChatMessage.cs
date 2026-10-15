using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Voyara.SharedKernel.Entities;

namespace Voyara.Domain.Entities.Guides
{
    /// <summary>
    /// Represents a message sent within a guide conversation thread.
    /// </summary>
    public sealed class ChatMessage : AuditableEntity
    {
        /// <summary>
        /// Gets the identifier of the conversation thread containing this message.
        /// </summary>
        public Guid ChatThreadId { get; private set; }

        /// <summary>
        /// Gets the identifier of the user who sent the message.
        /// </summary>
        public Guid UserId { get; private set; }

        /// <summary>
        /// Gets the text content of the message.
        /// </summary>
        public string Text { get; private set; } = string.Empty;

        /// <summary>
        /// Gets the date and time when the message was sent.
        /// </summary>
        public DateTime SentAt { get; private set; }

        /// <summary>
        /// Gets the date and time when the message was read, if applicable.
        /// </summary>
        public DateTime? ReadAt { get; private set; }

        /// <summary>
        /// Gets the conversation thread containing this message.
        /// </summary>
        public ChatThread? ChatThread { get; private set; }

        /// <summary>
        /// Gets the user who sent the message.
        /// </summary>
        public User? User { get; private set; }
    }
}
