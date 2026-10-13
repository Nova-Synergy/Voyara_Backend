using Voyara.SharedKernel.Entities;

namespace Voyara.Domain.Entities.Identity
{
    /// <summary>
    /// Represents a platform account (tourist, guide or admin) identified by its e-mail address.
    /// </summary>
    public sealed class User : AuditableEntity
    {
        /// <summary>
        /// Gets the full display name.
        /// </summary>
        public string FullName { get; private set; } = string.Empty;

        /// <summary>
        /// Gets the unique e-mail address.
        /// </summary>
        public string Email { get; private set; } = string.Empty;

        /// <summary>
        /// Gets the salted password hash; the raw password is never stored.
        /// </summary>
        public string PasswordHash { get; private set; } = string.Empty;

        /// <summary>
        /// Gets a value indicating whether the e-mail address was confirmed.
        /// </summary>
        public bool EmailConfirmed { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the account can sign in.
        /// </summary>
        public bool IsActive { get; private set; }

        /// <summary>
        /// Gets the roles granted to the user.
        /// </summary>
        public ICollection<Role> Roles { get; private set; } = new List<Role>();

        /// <summary>
        /// Gets the refresh tokens issued to the user.
        /// </summary>
        public ICollection<RefreshToken> RefreshTokens { get; private set; } = new List<RefreshToken>();

        /// <summary>
        /// Gets the tourist profile, when the user is a tourist.
        /// </summary>
        public TouristProfile? TouristProfile { get; private set; }
    }
}
