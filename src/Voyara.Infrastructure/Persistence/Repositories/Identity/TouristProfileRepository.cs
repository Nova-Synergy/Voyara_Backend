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

        /// <summary>Creates the repository.</summary>
        public TouristProfileRepository(VoyaraDbContext context)
        {
            _context = context;
        }

        /// <inheritdoc />
        public Task<TouristProfile?> GetByUserIdAsync(
            Guid userId,
            CancellationToken cancellationToken = default)
        {
            return _context.TouristProfiles
                .Include(p => p.User)
                .Include(p => p.Interests)
                .FirstOrDefaultAsync(p => p.UserId == userId, cancellationToken);
        }

        /// <inheritdoc />
        public Task<TouristProfile?> GetReadOnlyByUserIdAsync(
            Guid userId,
            CancellationToken cancellationToken = default)
        {
            return _context.TouristProfiles
                .AsNoTracking()
                .Include(p => p.User)
                .Include(p => p.Interests)
                .FirstOrDefaultAsync(p => p.UserId == userId, cancellationToken);
        }
    }
}
