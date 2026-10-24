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
        public Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
            => WithAggregate().FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

        /// <inheritdoc />
        public Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            var normalized = Normalize(email);
            return WithAggregate().FirstOrDefaultAsync(u => u.Email == normalized, cancellationToken);
        }

        /// <inheritdoc />
        public Task<User?> GetByRefreshTokenHashAsync(string tokenHash, CancellationToken cancellationToken = default)
            => WithAggregate().FirstOrDefaultAsync(
                u => u.RefreshTokens.Any(t => t.TokenHash == tokenHash), cancellationToken);

        /// <inheritdoc />
        public Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default)
        {
            var normalized = Normalize(email);
            return _context.Users.AsNoTracking().AnyAsync(u => u.Email == normalized, cancellationToken);
        }

        /// <inheritdoc />
        public void Add(User user) => _context.Users.Add(user);

        private IQueryable<User> WithAggregate()
            => _context.Users
                .Include(u => u.Roles)
                .Include(u => u.RefreshTokens);

        // Emails are stored trimmed and lower-case by the domain.
        private static string Normalize(string email) => (email ?? string.Empty).Trim().ToLowerInvariant();
    }
}