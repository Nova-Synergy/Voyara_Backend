using Voyara.SharedKernel.Entities;

namespace Voyara.Domain.Entities.Identity
{
    /// <summary>
    /// Represents a long-lived token used to obtain new access tokens; only its hash is stored.
    /// It is issued and revoked only through its <see cref="User"/>.
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

        private RefreshToken()
        {
        }

        /// <summary>
        /// Returns whether the token can still be exchanged for a new access token.
        /// </summary>
        public bool IsActive(DateTimeOffset now) => RevokedAt is null && now.UtcDateTime < ExpiresAt;

        internal static RefreshToken Issue(User user, string tokenHash, DateTimeOffset expiresAt, DateTimeOffset now)
        {
            ArgumentNullException.ThrowIfNull(user);
            ArgumentException.ThrowIfNullOrWhiteSpace(tokenHash);

            if (expiresAt <= now)
                throw new ArgumentException("A refresh token must expire in the future.", nameof(expiresAt));

            return new RefreshToken
            {
                User = user,
                UserId = user.Id,
                TokenHash = tokenHash,
                ExpiresAt = expiresAt.UtcDateTime
            };
        }

        internal void Revoke(DateTimeOffset now) => RevokedAt ??= now.UtcDateTime;
    }
}