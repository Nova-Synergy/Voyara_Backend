using Voyara.SharedKernel.Entities;

namespace Voyara.Domain.Entities.Identity
{
    /// <summary>
    /// Represents a platform account (tourist, guide or admin) identified by its e-mail address.
    /// It composes its refresh tokens: they are issued and revoked only through the user.
    /// </summary>
    public sealed class User : AuditableEntity
    {
        private const int MaxFullNameLength = 200;

        private readonly List<RefreshToken> _refreshTokens = [];
        private readonly List<Role> _roles = [];

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
        /// Read-only view of the roles granted to the user. Changed through <see cref="AssignRole"/>.
        /// </summary>
        public IReadOnlyCollection<Role> Roles => _roles.AsReadOnly();

        /// <summary>
        /// Read-only view of the refresh tokens issued to the user. Changed through <see cref="IssueRefreshToken"/>.
        /// </summary>
        public IReadOnlyCollection<RefreshToken> RefreshTokens => _refreshTokens.AsReadOnly();

        /// <summary>
        /// Gets the tourist profile, when the user is a tourist.
        /// </summary>
        public TouristProfile? TouristProfile { get; private set; }

        private User()
        {
        }

        /// <summary>
        /// Registers a new, active account whose e-mail address is not yet confirmed.
        /// Uniqueness of the e-mail address is checked by the caller.
        /// </summary>
        /// <param name="fullName">Display name, at most 200 characters.</param>
        /// <param name="email">E-mail address.</param>
        /// <param name="passwordHash">Salted password hash.</param>
        /// <returns>The created <see cref="User"/> instance.</returns>
        /// <exception cref="ArgumentException">An argument is missing or malformed.</exception>
        public static User Create(string fullName, string email, string passwordHash)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(passwordHash);

            return new User
            {
                FullName = NormalizeFullName(fullName),
                Email = NormalizeEmail(email),
                PasswordHash = passwordHash,
                IsActive = true
            };
        }

        /// <summary>Changes the display name.</summary>
        public void Rename(string fullName) => FullName = NormalizeFullName(fullName);

        /// <summary>Marks the e-mail address as confirmed.</summary>
        /// <exception cref="InvalidOperationException">The e-mail address is already confirmed.</exception>
        public void ConfirmEmail()
        {
            if (EmailConfirmed)
                throw new InvalidOperationException("The e-mail address is already confirmed.");

            EmailConfirmed = true;
        }

        /// <summary>Replaces the password hash and revokes every refresh token so other sessions end.</summary>
        public void ChangePassword(string newPasswordHash, DateTimeOffset now)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(newPasswordHash);

            PasswordHash = newPasswordHash;
            RevokeAllRefreshTokens(now);
        }

        /// <summary>Allows the account to sign in again.</summary>
        /// <exception cref="InvalidOperationException">The account is already active.</exception>
        public void Activate()
        {
            if (IsActive)
                throw new InvalidOperationException("The account is already active.");

            IsActive = true;
        }

        /// <summary>Blocks the account from signing in and revokes every refresh token.</summary>
        /// <exception cref="InvalidOperationException">The account is already deactivated.</exception>
        public void Deactivate(DateTimeOffset now)
        {
            if (!IsActive)
                throw new InvalidOperationException("The account is already deactivated.");

            IsActive = false;
            RevokeAllRefreshTokens(now);
        }

        /// <summary>Returns whether the user holds the role with the given name.</summary>
        public bool HasRole(string name)
            => _roles.Any(role => string.Equals(role.Name, name, StringComparison.OrdinalIgnoreCase));

        /// <summary>Grants a role; granting a role the user already holds has no effect.</summary>
        public void AssignRole(Role role)
        {
            ArgumentNullException.ThrowIfNull(role);

            if (!HasRole(role.Name))
                _roles.Add(role);
        }

        /// <summary>Withdraws a role; withdrawing a role the user does not hold has no effect.</summary>
        public void RemoveRole(Role role)
        {
            ArgumentNullException.ThrowIfNull(role);
            _roles.RemoveAll(r => string.Equals(r.Name, role.Name, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>Issues a refresh token (given as hash) to an active account.</summary>
        /// <exception cref="InvalidOperationException">The account is deactivated.</exception>
        public RefreshToken IssueRefreshToken(string tokenHash, DateTimeOffset expiresAt, DateTimeOffset now)
        {
            if (!IsActive)
                throw new InvalidOperationException("A deactivated account cannot receive refresh tokens.");

            var token = RefreshToken.Issue(this, tokenHash, expiresAt, now);
            _refreshTokens.Add(token);
            return token;
        }

        /// <summary>Finds a refresh token by hash that is neither revoked nor expired.</summary>
        public RefreshToken? FindActiveRefreshToken(string tokenHash, DateTimeOffset now)
            => _refreshTokens.Find(t => t.TokenHash == tokenHash && t.IsActive(now));

        /// <summary>Revokes one refresh token identified by its hash.</summary>
        /// <exception cref="InvalidOperationException">The user has no such token.</exception>
        public void RevokeRefreshToken(string tokenHash, DateTimeOffset now)
        {
            var token = _refreshTokens.Find(t => t.TokenHash == tokenHash)
                ?? throw new InvalidOperationException("The refresh token was not found.");

            token.Revoke(now);
        }

        /// <summary>Revokes every refresh token of the user.</summary>
        public void RevokeAllRefreshTokens(DateTimeOffset now) => _refreshTokens.ForEach(t => t.Revoke(now));

        private static string NormalizeFullName(string fullName)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(fullName);

            var normalized = fullName.Trim();
            if (normalized.Length > MaxFullNameLength)
                throw new ArgumentException($"Full name cannot exceed {MaxFullNameLength} characters.", nameof(fullName));

            return normalized;
        }

        private static string NormalizeEmail(string email)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(email);

            var normalized = email.Trim().ToLowerInvariant();
            var at = normalized.IndexOf('@');
            if (normalized.Length > 254 || at < 1 || at != normalized.LastIndexOf('@') || !normalized[(at + 1)..].Contains('.'))
                throw new ArgumentException("E-mail address is not valid.", nameof(email));

            return normalized;
        }
    }
}
