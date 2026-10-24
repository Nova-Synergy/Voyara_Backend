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
        public Task<TouristProfile?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
            => WithAggregate().FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

        /// <inheritdoc />
        public Task<TouristProfile?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
            => WithAggregate().FirstOrDefaultAsync(p => p.UserId == userId, cancellationToken);

        /// <inheritdoc />
        public Task<bool> ExistsForUserAsync(Guid userId, CancellationToken cancellationToken = default)
            => _context.TouristProfiles.AsNoTracking().AnyAsync(p => p.UserId == userId, cancellationToken);

        /// <inheritdoc />
        public void Add(TouristProfile profile) => _context.TouristProfiles.Add(profile);

        private IQueryable<TouristProfile> WithAggregate()
            => _context.TouristProfiles
                .Include(p => p.Interests)
                .Include(p => p.VisitHistories)
                .AsSplitQuery();
    }
}