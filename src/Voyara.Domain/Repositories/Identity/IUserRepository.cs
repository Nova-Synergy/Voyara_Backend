using Voyara.Domain.Entities.Identity;

namespace Voyara.Domain.Repositories.Identity
{

    /// <summary>
    /// Persistence contract for the <see cref="User"/> aggregate (roles, refresh tokens, tourist profile).
    /// Changes are saved by the unit of work, not by the repository.
    /// </summary>
    public interface IUserRepository
    {
        /// <summary>
        /// Gets a tracked user with roles and refresh tokens, optionally restricted to a role.
        /// </summary>
        Task<User?> GetByIdAsync(Guid id, string? requiredRole = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets a read-only user with roles, optionally restricted to a role.
        /// </summary>
        Task<User?> GetByIdReadOnlyAsync(Guid id, string? requiredRole = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Searches users by name or e-mail, optionally by role, with paging (read-only).
        /// </summary>
        Task<(IReadOnlyList<User> Items, int TotalCount)> SearchAsync(
            string? searchTerm, string? role, int page, int pageSize, CancellationToken cancellationToken = default);

        /// <summary>
        /// Checks e-mail uniqueness, including soft-deleted accounts (their e-mail stays reserved).
        /// </summary>
        Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default);

        /// <summary>
        /// Starts tracking a new user.
        /// </summary>
        void Add(User user);
    }
}
