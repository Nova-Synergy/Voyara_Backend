using Voyara.Domain.Entities.Identity;

namespace Voyara.Domain.Repositories.Identity
{

    /// <summary>
    /// Persistence contract for <see cref="Role"/>.
    /// </summary>
    public interface IRoleRepository
    {
        /// <summary>
        /// Gets a tracked role by name (case-insensitive), or null.
        /// </summary>
        Task<Role?> GetByNameAsync(string name, CancellationToken cancellationToken = default);

        /// <summary>
        /// Starts tracking a new role.
        /// </summary>
        void Add(Role role);
    }
}
