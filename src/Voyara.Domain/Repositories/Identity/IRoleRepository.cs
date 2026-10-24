using Voyara.Domain.Entities.Identity;

namespace Voyara.Domain.Repositories.Identity
{

    /// <summary>
    /// Persistence contract for <see cref="Role"/>.
    /// </summary>
    public interface IRoleRepository
    {
        /// <summary>
        /// Gets a role by identifier, or <see langword="null"/>.
        /// </summary>
        Task<Role?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets a role by name (case-insensitive), or <see langword="null"/>.
        /// </summary>
        Task<Role?> GetByNameAsync(string name, CancellationToken cancellationToken = default);

        /// <summary>
        /// Lists all roles ordered by name.
        /// </summary>
        Task<IReadOnlyList<Role>> ListAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Checks whether a role with the name exists.
        /// </summary>
        Task<bool> ExistsAsync(string name, CancellationToken cancellationToken = default);

        /// <summary>
        /// Registers a new role for insertion.
        /// </summary>
        void Add(Role role);
    }
}
