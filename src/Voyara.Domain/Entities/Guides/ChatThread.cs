using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Voyara.SharedKernel.Entities;

namespace Voyara.Domain.Entities.Guides
{
    /// <summary>
    /// Represents the conversation thread associated with a guide request.
    /// </summary>
    public class ChatThread : AuditableEntity
    {

        /// <summary>
        /// Gets the identifier of the <see cref="GuideRequest"/> that owns this chat thread.
        /// </summary>
        public Guid GuideRequestId { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the chat thread is closed and no longer accepting messages.
        /// </summary>
        public bool IsClosed { get; private set; }

        /// <summary>
        /// Navigation property to the collection of messages in the chat thread.
        /// </summary>
        public ICollection<ChatMessage> Messages { get; private set; } = new List<ChatMessage>();

        /// <summary>
        /// Navigation property to the associated guide request.
        /// </summary>
        public GuideRequest GuideRequest { get; private set; } = null!;
    }
}
