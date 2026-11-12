using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Voyara.Domain.Entities.AiAssistant;
using Voyara.SharedKernel.Entities;

namespace Voyara.Domain.Entities.Guides
{
    /// <summary>
    /// Represents the conversation thread associated with a guide request.
    /// </summary>
    public sealed class ChatThread : AuditableEntity
    {
        /// <summary>
        /// Gets the collection of messages in the conversation thread.
        /// </summary>
        private readonly List<ChatMessage> _messages = new();

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

        /// <summary>Read-only view. Messages are added only through <see cref="PostMessage"/>.</summary>
        public IReadOnlyCollection<ChatMessage> Messages => _messages.AsReadOnly();

        /// <summary>
        /// Initializes a new instance of the ChatThread class. 
        /// </summary>
        /// <remarks>This constructor is private and is intended for internal use only. Instances of
        /// ChatThread cannot be created directly by consumers.</remarks>
        private ChatThread()
        {
        }

        /// <summary>
        /// Creates a new chat thread for the specified guide request.
        /// </summary>
        /// <param name="guideRequestId">The identifier of the guide request.</param>
        /// <returns>The created <see cref="ChatThread"/> instance.</returns>
        /// <exception cref="ArgumentException"></exception>
        internal static ChatThread Create(Guid guideRequestId)
        {
            if (guideRequestId == Guid.Empty)
                throw new ArgumentException("A guide request is required.", nameof(guideRequestId));

            return new ChatThread { GuideRequestId = guideRequestId };
        }

        /// <summary>
        /// Posts a message. Closed threads reject messages, and only the two parties of the request may post.
        /// The caller supplies the parties' user ids (tourist = request.UserId, guide = guideProfile.UserId).
        /// </summary>
        public ChatMessage PostMessage(Guid senderId, string text, Guid touristUserId, Guid guideUserId, DateTimeOffset now)
        {
            if (IsClosed)
                throw new InvalidOperationException("Cannot post a message to a closed thread.");

            EnsureParticipant(senderId, touristUserId, guideUserId);

            var message = ChatMessage.Create(Id, senderId, text, now);
            _messages.Add(message);
            return message;
        }

        /// <summary>
        /// Marks a message as read by the other party. Reading is still allowed on a closed thread.
        /// </summary>
        internal void MarkMessageAsRead(Guid messageId, Guid readerId, Guid touristUserId, Guid guideUserId, DateTimeOffset now)
        {
            EnsureParticipant(readerId, touristUserId, guideUserId);

            var message = _messages.FirstOrDefault(m => m.Id == messageId)
                ?? throw new InvalidOperationException("Message was not found in this thread.");

            message.MarkAsRead(readerId, now);
        }

        /// <summary>
        /// Closes the thread. Internal: GuideRequest closes its threads when it is cancelled or completed.
        /// </summary>
        internal void Close() => IsClosed = true;

        /// <summary>
        /// Ensures that the specified user is a valid participant in the chat
        /// and that all user identifiers are valid.
        /// </summary>
        /// <param name="userId">The identifier of the user attempting to access the chat.</param>
        /// <param name="touristUserId">The identifier of the tourist associated with the request.</param>
        /// <param name="guideUserId">The identifier of the guide associated with the request.</param>
        /// <exception cref="ArgumentException">
        /// Thrown when any of the specified user identifiers is <see cref="Guid.Empty"/>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the specified user is neither the tourist nor the guide associated with the request.
        /// </exception>
        private static void EnsureParticipant(Guid userId, Guid touristUserId, Guid guideUserId)
        {
            if (userId == Guid.Empty || touristUserId == Guid.Empty || guideUserId == Guid.Empty)
                throw new ArgumentException("User ids must not be empty.");

            if (userId != touristUserId && userId != guideUserId)
                throw new InvalidOperationException("Only the two parties of the request can use this chat.");
        }
    }
}