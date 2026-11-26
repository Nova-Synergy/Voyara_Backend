using Microsoft.EntityFrameworkCore;
using Voyara.Domain.Entities.Identity;
using Voyara.Domain.Repositories.Identity;

namespace Voyara.Infrastructure.Persistence.Repositories.Identity
{
    /// <summary>
    /// EF Core implementation of <see cref="IRoleRepository"/>.
    /// </summary>
    public sealed class RoleRepository : IRoleRepository
    {
        private readonly VoyaraDbContext _context;

        /// <summary>Creates the repository.</summary>
        public RoleRepository(VoyaraDbContext context) => _context = context;

        /// <inheritdoc />
        public Task<Role?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
        {
            var normalized = name.Trim().ToLower();

            return _context.Roles
                .FirstOrDefaultAsync(r => r.Name.ToLower() == normalized, cancellationToken);
        }
    }
}
