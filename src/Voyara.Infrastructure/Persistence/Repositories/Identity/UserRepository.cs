using Microsoft.EntityFrameworkCore;
using Voyara.Domain.Entities.Identity;
using Voyara.Domain.Repositories.Identity;

namespace Voyara.Infrastructure.Persistence.Repositories.Identity
{

    /// <summary>
    /// EF Core implementation of <see cref="IUserRepository"/>.
    /// </summary>
    public sealed class UserRepository : IUserRepository
    {
        private readonly VoyaraDbContext _context;

        /// <summary>
        /// Creates the repository.
        /// </summary>
        public UserRepository(VoyaraDbContext context) => _context = context;

        /// <inheritdoc />
        public Task<User?> GetByIdAsync(Guid id, string? requiredRole = null, CancellationToken cancellationToken = default)
        {
            IQueryable<User> query = _context.Users.Include(u => u.Roles).Include(u => u.RefreshTokens);
            if (requiredRole is not null)
                query = query.Where(u => u.Roles.Any(r => r.Name == requiredRole));

            return query.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
        }

        /// <inheritdoc />
        public Task<User?> GetByIdReadOnlyAsync(Guid id, string? requiredRole = null, CancellationToken cancellationToken = default)
        {
            IQueryable<User> query = _context.Users.AsNoTracking().Include(u => u.Roles);
            if (requiredRole is not null)
                query = query.Where(u => u.Roles.Any(r => r.Name == requiredRole));

            return query.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
        }

        /// <inheritdoc />
        public async Task<(IReadOnlyList<User> Items, int TotalCount)> SearchAsync(
            string? searchTerm, string? role, int page, int pageSize, CancellationToken cancellationToken = default)
        {
            IQueryable<User> query = _context.Users.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(role))
                query = query.Where(u => u.Roles.Any(r => r.Name == role));

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var term = searchTerm.Trim().ToLower();
                query = query.Where(u => u.FullName.ToLower().Contains(term) || u.Email.Contains(term));
            }

            var total = await query.CountAsync(cancellationToken);
            var items = await query
                .Include(u => u.Roles)
                .OrderBy(u => u.FullName).ThenBy(u => u.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return (items, total);
        }

        /// <inheritdoc />
        public Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default)
        {
            // Emails are stored trimmed and lower-case; soft-deleted accounts keep their e-mail reserved.
            var normalized = (email ?? string.Empty).Trim().ToLowerInvariant();
            return _context.Users.IgnoreQueryFilters().AsNoTracking()
                .AnyAsync(u => u.Email == normalized, cancellationToken);
        }

        /// <inheritdoc />
        public void Add(User user) => _context.Users.Add(user);
    }
}