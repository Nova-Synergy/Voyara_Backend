using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Voyara.Domain.Entities.Guides
{
    /// <summary>
    /// Represents a message sent within a guide conversation thread.
    /// </summary>
    public class ChatMessage
    {
        /// <summary>
        /// Gets the primary key of the chat message.
        /// </summary>
        public long Id { get; private set; }

        /// <summary>
        /// Gets the text content of the message.
        /// </summary>
        public string Text { get; private set; } = string.Empty;

        /// <summary>
        /// Gets the date and time when the message was sent.
        /// </summary>
        public DateTime SentAt { get; private set; }

        /// <summary>
        /// Gets the date and time when the message was read, if available.
        /// </summary>
        public DateTime? ReadAt { get; private set; }

        /// <summary>
        /// Gets the identifier of the associated <see cref="ChatThread"/>.
        /// </summary>
        public Guid ChatThreadId { get; private set; }

        /// <summary>
        /// Gets the identifier of the user who sent the message.
        /// </summary>
        public Guid SenderId { get; private set; }
    }
}
