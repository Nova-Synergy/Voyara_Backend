using Microsoft.EntityFrameworkCore;
using Voyara.Domain.Entities.Identity;
using Voyara.Domain.Repositories.Identity;

namespace Voyara.Infrastructure.Persistence.Repositories.Identity
{

    /// <summary>
    /// EF Core implementation of <see cref="ITouristProfileRepository"/>.
    /// </summary>
    public sealed class TouristProfileRepository : ITouristProfileRepository
    {
        private readonly VoyaraDbContext _context;

        /// <summary>
        /// Creates the repository.
        /// </summary>
        public TouristProfileRepository(VoyaraDbContext context) => _context = context;

        /// <inheritdoc />
        public Task<TouristProfile?> GetByUserIdAsync(Guid userId, bool asNoTracking = false, CancellationToken cancellationToken = default)
        {
            IQueryable<TouristProfile> query = _context.TouristProfiles;
            if (asNoTracking)
                query = query.AsNoTracking();

            return query
                .Include(p => p.User)
                .Include(p => p.Interests)
                .FirstOrDefaultAsync(p => p.UserId == userId, cancellationToken);
        }

        /// <inheritdoc />
        public async Task<(IReadOnlyList<TouristProfile> Items, int TotalCount)> SearchAsync(
            string? searchTerm, int page, int pageSize, CancellationToken cancellationToken = default)
        {
            IQueryable<TouristProfile> query = _context.TouristProfiles.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var term = searchTerm.Trim().ToLower();
                query = query.Where(p => p.User.FullName.ToLower().Contains(term) || p.User.Email.Contains(term));
            }

            var total = await query.CountAsync(cancellationToken);
            var items = await query
                .Include(p => p.User)
                .Include(p => p.Interests)
                .OrderBy(p => p.User.FullName).ThenBy(p => p.UserId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return (items, total);
        }

        /// <inheritdoc />
        public void Add(TouristProfile profile) => _context.TouristProfiles.Add(profile);
    }
}