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

        /// <summary>Creates the repository.</summary>
        public UserRepository(VoyaraDbContext context) => _context = context;

        /// <inheritdoc />
        public Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
            => _context.Users
                .Include(u => u.Roles)
                .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

        /// <inheritdoc />
        public Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            var normalized = email.Trim().ToLowerInvariant();

            return _context.Users
                .Include(u => u.Roles)
                .FirstOrDefaultAsync(u => u.Email == normalized, cancellationToken);
        }

        /// <inheritdoc />
        public Task<User?> GetByRefreshTokenHashAsync(string tokenHash, CancellationToken cancellationToken = default)
            => _context.Users
                .Include(u => u.Roles)
                .Include(u => u.RefreshTokens)
                .FirstOrDefaultAsync(u => u.RefreshTokens.Any(t => t.TokenHash == tokenHash), cancellationToken);

        /// <inheritdoc />
        public Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            var normalized = email.Trim().ToLowerInvariant();

            // Soft-deleted accounts still own their e-mail address because of the unique index.
            return _context.Users
                .IgnoreQueryFilters()
                .AnyAsync(u => u.Email == normalized, cancellationToken);
        }

        /// <inheritdoc />
        public void Add(User user) => _context.Users.Add(user);
    }
}
