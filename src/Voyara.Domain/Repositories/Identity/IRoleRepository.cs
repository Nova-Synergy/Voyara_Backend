using Voyara.Domain.Entities.Identity;

namespace Voyara.Domain.Repositories.Identity
{

    /// <summary>
    /// Persistence contract for <see cref="Role"/>.
    /// </summary>
    public interface IRoleRepository
    {
        /// <summary>Finds a role by name (case-insensitive).</summary>
        Task<Role?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    }
}
