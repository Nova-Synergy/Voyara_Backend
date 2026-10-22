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

        /// <summary>
        /// Creates the repository.
        /// </summary>
        public InterestRepository(VoyaraDbContext context) => _context = context;

        /// <inheritdoc />
        public Task<Interest?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
            => _context.Interests.FirstOrDefaultAsync(i => i.Id == id, cancellationToken);

        /// <inheritdoc />
        public Task<Interest?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
        {
            var normalized = Normalize(code);
            return _context.Interests.FirstOrDefaultAsync(i => i.Code == normalized, cancellationToken);
        }

        /// <inheritdoc />
        public async Task<IReadOnlyList<Interest>> GetByIdsAsync(IEnumerable<int> ids, CancellationToken cancellationToken = default)
        {
            var idList = ids.Distinct().ToArray();
            if (idList.Length == 0)
            {
                return [];
            }

            return await _context.Interests.Where(i => idList.Contains(i.Id)).ToListAsync(cancellationToken);
        }

        /// <inheritdoc />
        public async Task<IReadOnlyList<Interest>> ListAsync(CancellationToken cancellationToken = default)
            => await _context.Interests.AsNoTracking().OrderBy(i => i.Code).ToListAsync(cancellationToken);

        /// <inheritdoc />
        public Task<bool> ExistsAsync(string code, CancellationToken cancellationToken = default)
        {
            var normalized = Normalize(code);
            return _context.Interests.AsNoTracking().AnyAsync(i => i.Code == normalized, cancellationToken);
        }

        /// <inheritdoc />
        public void Add(Interest interest) => _context.Interests.Add(interest);

        // Interest codes are stored trimmed and lower-case by the domain.
        private static string Normalize(string code) => (code ?? string.Empty).Trim().ToLowerInvariant();
    }
}
