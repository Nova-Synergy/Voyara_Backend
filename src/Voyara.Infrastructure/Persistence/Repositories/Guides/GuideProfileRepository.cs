using Microsoft.EntityFrameworkCore;
using Voyara.Domain.Entities.Guides;
using Voyara.Domain.Repositories.Guides;

namespace Voyara.Infrastructure.Persistence.Repository.Guides
{
    /// <summary>
    /// Provides persistence operations for guide profiles.
    /// </summary>
    public sealed class GuideProfileRepository : IGuideProfileRepository
    {
        private readonly VoyaraDbContext _context;

        public GuideProfileRepository(VoyaraDbContext context) => _context = context;

        public void Add(GuideProfile profile) => _context.Set<GuideProfile>().Add(profile);

        public Task<bool> ExistsByUserIdAsync(Guid userId, CancellationToken cancellationToken = default) =>
            _context.Set<GuideProfile>().AnyAsync(p => p.UserId == userId && !p.IsDeleted, cancellationToken);

        public Task<GuideProfile?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
            _context.Set<GuideProfile>().FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted, cancellationToken);

        public Task<GuideProfile?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default) =>
            _context.Set<GuideProfile>().FirstOrDefaultAsync(p => p.UserId == userId && !p.IsDeleted, cancellationToken);

        public Task<GuideProfile?> GetByUserIdWithDetailsAsync(Guid userId, CancellationToken cancellationToken = default) =>
            // Child rows removed through the aggregate are soft-deleted by the audit interceptor and the
            // child configurations have no query filter, so deleted rows are excluded explicitly here.
            _context.Set<GuideProfile>()
                .Include(p => p.Languages.Where(l => !l.IsDeleted))
                .Include(p => p.Expertise.Where(e => !e.IsDeleted))
                .Include(p => p.Availability.Where(a => !a.IsDeleted))
                .Include(p => p.Pricing.Where(x => !x.IsDeleted))
                .AsSplitQuery()
                .FirstOrDefaultAsync(p => p.UserId == userId && !p.IsDeleted, cancellationToken);
    }
}
