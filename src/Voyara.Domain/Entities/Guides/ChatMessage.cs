using Voyara.Domain.Entities.Identity;
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

        /// <summary>
        /// Initializes a new instance of the ChatMessage class. This constructor is intended for internal use only.
        /// </summary>
        /// <remarks>This constructor is typically used by serialization frameworks or within the class
        /// itself. It is not intended to be called directly by application code.</remarks>
        private ChatMessage()
        {
        }

        /// <summary>
        /// Creates a new chat message instance for the specified thread, sender, and message text at the given time.   
        /// </summary>
        /// <param name="threadId">The unique identifier of the chat thread to which the message belongs. Must not be <see cref="Guid.Empty"/>.</param>
        /// <param name="senderId">The unique identifier of the user sending the message. Must not be <see cref="Guid.Empty"/>.</param>
        /// <param name="text">The text content of the message. Must not be null, empty, or consist only of white-space characters. The
        /// trimmed text cannot exceed 4000 characters.</param>
        /// <param name="now">The date and time, in UTC, when the message is sent.</param>
        /// <returns>A new <see cref="ChatMessage"/> instance initialized with the specified thread, sender, text, and timestamp.</returns>
        /// <exception cref="ArgumentException">Thrown if <paramref name="threadId"/> or <paramref name="senderId"/> is <see cref="Guid.Empty"/>, if
        /// <paramref name="text"/> is null, empty, or white-space, or if the trimmed <paramref name="text"/> exceeds
        /// 4000 characters.</exception>
        internal static ChatMessage Create(Guid threadId, Guid senderId, string text, DateTimeOffset now)
        {
            if (threadId == Guid.Empty)
                throw new ArgumentException("A chat thread is required.", nameof(threadId));
            
            if (senderId == Guid.Empty)
                throw new ArgumentException("A sender is required.", nameof(senderId));
            
            if (string.IsNullOrWhiteSpace(text))
                throw new ArgumentException("Message text is required.", nameof(text));
            
            if (text.Trim().Length > 4000)
                throw new ArgumentException("Message text cannot exceed 4000 characters.", nameof(text));

            return new ChatMessage
            {
                ChatThreadId = threadId,
                UserId = senderId,
                Text = text.Trim(),
                SentAt = now.UtcDateTime
            };
        }

        /// <summary>
        /// Marks the message as read by someone other than its sender. Idempotent.
        /// Internal: callers go through <see cref="ChatThread.MarkMessageAsRead"/>, which verifies the reader is a party to the request.
        /// </summary>
        internal void MarkAsRead(Guid readerId, DateTimeOffset now)
        {
            if (readerId == Guid.Empty)
                throw new ArgumentException("A reader is required.", nameof(readerId));
            if (readerId == UserId)
                throw new InvalidOperationException("A sender cannot mark their own message as read.");

            ReadAt ??= now.UtcDateTime;
        }
    }
}
