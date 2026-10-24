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

        /// <summary>
        /// Creates the repository.
        /// </summary>
        public RoleRepository(VoyaraDbContext context) => _context = context;

        /// <inheritdoc />
        public Task<Role?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
            => _context.Roles.FirstOrDefaultAsync(r => r.Id == id, cancellationToken);

        /// <inheritdoc />
        public Task<Role?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
        {
            var normalized = Normalize(name);
            return _context.Roles.FirstOrDefaultAsync(r => r.Name.ToLower() == normalized, cancellationToken);
        }

        /// <inheritdoc />
        public async Task<IReadOnlyList<Role>> ListAsync(CancellationToken cancellationToken = default)
            => await _context.Roles.AsNoTracking().OrderBy(r => r.Name).ToListAsync(cancellationToken);

        /// <inheritdoc />
        public Task<bool> ExistsAsync(string name, CancellationToken cancellationToken = default)
        {
            var normalized = Normalize(name);
            return _context.Roles.AsNoTracking().AnyAsync(r => r.Name.ToLower() == normalized, cancellationToken);
        }

        /// <inheritdoc />
        public void Add(Role role) => _context.Roles.Add(role);

        private static string Normalize(string name) => (name ?? string.Empty).Trim().ToLowerInvariant();
    }
}