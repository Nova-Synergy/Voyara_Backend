using Microsoft.EntityFrameworkCore;
using Voyara.Domain.Entities.Guides;
using Voyara.Domain.Enums;
using Voyara.Domain.Repositories.Guides;

namespace Voyara.Infrastructure.Persistence.Repositories.Guides
{
    /// <summary>EF Core implementation of <see cref="IGuideRequestRepository"/>.</summary>
    public sealed class GuideRequestRepository : IGuideRequestRepository
    {
        private readonly VoyaraDbContext _context;

        public GuideRequestRepository(VoyaraDbContext context) => _context = context;

        public void Add(GuideRequest request) => _context.Set<GuideRequest>().Add(request);

        public Task<GuideRequest?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
            _context.Set<GuideRequest>()
                .Include(r => r.ChatThreads.Where(t => !t.IsDeleted))
                .FirstOrDefaultAsync(r => r.Id == id && !r.IsDeleted, cancellationToken);

        public Task<GuideRequest?> GetByIdWithReviewAsync(Guid id, CancellationToken cancellationToken = default) =>
            _context.Set<GuideRequest>()
                .Include(r => r.GuideReview)
                .FirstOrDefaultAsync(r => r.Id == id && !r.IsDeleted, cancellationToken);

        public Task<GuideRequest?> GetByChatThreadIdAsync(
            Guid chatThreadId, Guid? messageId = null, CancellationToken cancellationToken = default)
        {
            var query = _context.Set<GuideRequest>()
                .Include(r => r.GuideProfile)
                .Where(r => !r.IsDeleted && r.ChatThreads.Any(t => t.Id == chatThreadId && !t.IsDeleted));

            if (messageId is { } id)
            {
                return query
                    .Include(r => r.ChatThreads.Where(t => t.Id == chatThreadId))
                        .ThenInclude(t => t.Messages.Where(m => m.Id == id && !m.IsDeleted))
                    .FirstOrDefaultAsync(cancellationToken);
            }

            return query
                .Include(r => r.ChatThreads.Where(t => t.Id == chatThreadId))
                .FirstOrDefaultAsync(cancellationToken);
        }

        public Task<Guid?> GetGuideProfileIdAsync(Guid requestId, CancellationToken cancellationToken = default) =>
            _context.Set<GuideRequest>()
                .AsNoTracking()
                .Where(r => r.Id == requestId && !r.IsDeleted)
                .Select(r => (Guid?)r.GuideProfileId)
                .FirstOrDefaultAsync(cancellationToken);

        public Task<bool> ExistsActiveAsync(
            Guid userId, Guid guideProfileId, DateOnly date, TimeOnly startTime, CancellationToken cancellationToken = default) =>
            _context.Set<GuideRequest>()
                .AsNoTracking()
                .AnyAsync(r => !r.IsDeleted
                    && r.UserId == userId
                    && r.GuideProfileId == guideProfileId
                    && r.Date == date
                    && r.StartTime == startTime
                    && (r.Status == RequestStatus.Pending || r.Status == RequestStatus.Accepted),
                    cancellationToken);

        public async Task<IReadOnlyList<GuideRequestSlot>> ListTakenSlotsAsync(
            Guid guideProfileId, DateOnly date, CancellationToken cancellationToken = default) =>
            await _context.Set<GuideRequest>()
                .AsNoTracking()
                .Where(r => !r.IsDeleted
                    && r.GuideProfileId == guideProfileId
                    && r.Date == date
                    && (r.Status == RequestStatus.Accepted || r.Status == RequestStatus.Completed))
                .Select(r => new GuideRequestSlot(r.StartTime, r.DurationHours))
                .ToListAsync(cancellationToken);

        public async Task<IReadOnlyList<GuideRequest>> ListExpiredPendingAsync(
            DateTimeOffset now, int batchSize, CancellationToken cancellationToken = default) =>
            await _context.Set<GuideRequest>()
                .Where(r => !r.IsDeleted && r.Status == RequestStatus.Pending && r.ExpiresAt <= now)
                .OrderBy(r => r.ExpiresAt)
                .ThenBy(r => r.Id)
                .Take(batchSize)
                .ToListAsync(cancellationToken);

        public Task AcquireGuideLockAsync(Guid guideProfileId, CancellationToken cancellationToken = default)
        {
            // Transaction-scoped PostgreSQL advisory lock: released automatically on commit or rollback.
            var key = $"voyara-guide:{guideProfileId:N}";
            return _context.Database.ExecuteSqlInterpolatedAsync(
                $"SELECT pg_advisory_xact_lock(hashtextextended({key}, 0))", cancellationToken);
        }
    }
}
