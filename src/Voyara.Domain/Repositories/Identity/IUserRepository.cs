using Voyara.Domain.Entities.Identity;

namespace Voyara.Domain.Repositories.Identity
{

    /// <summary>
    /// Persistence contract for the <see cref="User"/> aggregate (roles, refresh tokens, tourist profile).
    /// </summary>
    public interface IUserRepository
    {
        /// <summary>Loads a user with its roles, tracked for changes.</summary>
        Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

        /// <summary>Loads a user by e-mail address (case-insensitive) with its roles, tracked for changes.</summary>
        Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);

        /// <summary>
        /// Loads the user owning the refresh token with the given hash, whatever its state (active, revoked
        /// or expired), together with the roles and all refresh tokens so reuse can be detected.
        /// </summary>
        Task<User?> GetByRefreshTokenHashAsync(string tokenHash, CancellationToken cancellationToken = default);

        /// <summary>Returns whether an account (including a soft-deleted one) already uses the e-mail address.</summary>
        Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default);

        /// <summary>Starts tracking a new user.</summary>
        void Add(User user);
    }
}
