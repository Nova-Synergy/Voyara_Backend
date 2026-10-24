using Voyara.Domain.Entities.Identity;

namespace Voyara.Domain.Repositories.Identity
{

    /// <summary>
    /// Persistence contract for the <see cref="User"/> aggregate (roles, refresh tokens, tourist profile).
    /// </summary>
    public interface IUserRepository
    {
        /// <summary>
        /// Gets a user with roles and refresh tokens, or <see langword="null"/>.
        /// </summary>
        Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets a user by e-mail address (case-insensitive) with roles and refresh tokens, or <see langword="null"/>.
        /// </summary>
        Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the user that owns the refresh token with the given hash, or <see langword="null"/>.
        /// </summary>
        Task<User?> GetByRefreshTokenHashAsync(string tokenHash, CancellationToken cancellationToken = default);

        /// <summary>
        /// Checks whether an account with the e-mail address already exists.
        /// </summary>
        Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default);

        /// <summary>
        /// Registers a new user for insertion.
        /// </summary>
        void Add(User user);
    }
}
