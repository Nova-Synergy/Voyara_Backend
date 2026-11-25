using Microsoft.EntityFrameworkCore;
using Voyara.Domain.Entities.Identity;
using Voyara.Domain.Repositories.Identity;

namespace Voyara.Infrastructure.Persistence.Repositories.Identity
{

    /// <summary>
    /// EF Core implementation of <see cref="IInterestRepository"/>.
    /// </summary>
    public sealed class InterestRepository : IInterestRepository
    {
        private readonly VoyaraDbContext _context;

        /// <summary>Creates the repository.</summary>
        public InterestRepository(VoyaraDbContext context)
        {
            _context = context;
        }

        /// <inheritdoc />
        public async Task<IReadOnlyList<Interest>> GetAllAsync(
            CancellationToken cancellationToken = default)
        {
            return await _context.Interests
                .AsNoTracking()
                .OrderBy(i => i.Code)
                .ToListAsync(cancellationToken);
        }

        /// <inheritdoc />
        public async Task<IReadOnlyList<Interest>> GetByCodesAsync(
            IReadOnlyCollection<string> codes,
            CancellationToken cancellationToken = default)
        {
            var list = codes.ToList();

            return await _context.Interests
                .Where(i => list.Contains(i.Code))
                .ToListAsync(cancellationToken);
        }
    }
}
