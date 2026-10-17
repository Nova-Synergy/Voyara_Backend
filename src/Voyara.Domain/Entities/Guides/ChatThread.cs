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
    public sealed class ChatThread : AuditableEntity
    {
        /// <summary>
        /// Gets the identifier of the associated guide request.
        /// </summary>
        public Guid GuideRequestId { get; private set; }

        /// <summary>
        /// Gets whether the conversation thread is closed.
        /// </summary>
        public bool IsClosed { get; private set; }

        /// <summary>
        /// Gets the guide request associated with this conversation.
        /// </summary>
        public GuideRequest GuideRequest { get; private set; } = null!;

        /// <summary>
        /// Gets the messages belonging to this conversation thread.
        /// </summary>
        public ICollection<ChatMessage> Messages { get; private set; } = new List<ChatMessage>();
    }
}
