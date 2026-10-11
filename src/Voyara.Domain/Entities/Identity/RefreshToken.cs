using Voyara.SharedKernel.Entities;

namespace Voyara.Domain.Entities.Identity
{
    /// <summary>
    /// Represents a long-lived token used to obtain new access tokens; only its hash is stored.
    /// </summary>
    public sealed class RefreshToken : BaseEntity
    {
        /// <summary>
        /// Gets the hash of the token value; the raw token is never stored.
        /// </summary>
        public string TokenHash { get; private set; } = string.Empty;

        /// <summary>
        /// Gets the expiry timestamp (UTC).
        /// </summary>
        public DateTime ExpiresAt { get; private set; }

        /// <summary>
        /// Gets the timestamp (UTC) when the token was revoked, if it was.
        /// </summary>
        public DateTime? RevokedAt { get; private set; }

        /// <summary>
        /// Gets the identifier of the owning user.
        /// </summary>
        public Guid UserId { get; private set; }

        /// <summary>
        /// Gets the user that owns this token.
        /// </summary>
        public User User { get; private set; } = null!;
    }
}