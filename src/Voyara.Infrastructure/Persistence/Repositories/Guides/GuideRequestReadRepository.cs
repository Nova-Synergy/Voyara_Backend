using Microsoft.EntityFrameworkCore;
using Voyara.Application.DTOs;
using Voyara.Application.DTOs.Guides;
using Voyara.Application.Guides;
using Voyara.Domain.Entities.Guides;
using Voyara.Domain.Enums;

namespace Voyara.Infrastructure.Persistence.Repositories.Guides
{
    public sealed class GuideRequestReadRepository : IGuideRequestReadRepository 
    {
        private readonly VoyaraDbContext _context;

        public GuideRequestReadRepository(VoyaraDbContext context) => _context = context;

        public Task<GuideBookingContext?> GetBookingContextAsync(
            Guid guideProfileId, CancellationToken cancellationToken = default) =>
            _context.Set<GuideProfile>()
                .AsNoTracking()
                .Where(p => p.Id == guideProfileId && !p.IsDeleted)
                .Select(p => new GuideBookingContext(
                    p.Id,
                    p.UserId,
                    p.Status,
                    p.City!.Country.TimeZone,
                    p.Availability.Where(a => !a.IsDeleted)
                        .Select(a => new GuideAvailabilityWindow(a.Day, a.StartTime, a.EndTime)).ToList(),
                    p.TimeOff.Where(t => !t.IsDeleted).Select(t => t.Date).ToList()))
                .FirstOrDefaultAsync(cancellationToken);

        public Task<PagedResult<GuideRequestListItemDto>> ListForTouristAsync(
            Guid touristUserId, RequestStatus? status, int page, int pageSize, CancellationToken cancellationToken = default) =>
            ListAsync(r => r.UserId == touristUserId, status, page, pageSize, cancellationToken);

        public Task<PagedResult<GuideRequestListItemDto>> ListForGuideAsync(
            Guid guideUserId, RequestStatus? status, int page, int pageSize, CancellationToken cancellationToken = default) =>
            ListAsync(r => r.GuideProfile!.UserId == guideUserId, status, page, pageSize, cancellationToken);

        public Task<GuideRequestAccessModel?> GetRequestAsync(Guid requestId, CancellationToken cancellationToken = default) =>
            _context.Set<GuideRequest>()
                .AsNoTracking()
                .Where(r => r.Id == requestId && !r.IsDeleted)
                .Select(r => new GuideRequestAccessModel(
                    new GuideRequestDetailsDto(
                        r.Id,
                        r.GuideProfileId,
                        r.GuideProfile!.User!.FullName,
                        r.UserId,
                        r.User!.FullName,
                        r.Date,
                        r.StartTime,
                        r.DurationHours,
                        r.PartySize,
                        r.PreferredLanguage,
                        r.SpecialRequirements,
                        r.Status,
                        r.RejectionReason,
                        r.ExpiresAt,
                        r.CreatedAt,
                        r.GuideSession == null
                            ? null
                            : new GuideSessionDto(
                                r.GuideSession.Id,
                                r.GuideSession.GuideRequestId,
                                r.GuideSession.MeetingLocation,
                                r.GuideSession.AgreedPrice,
                                r.GuideSession.Currency),
                        r.ChatThreads.Where(t => !t.IsDeleted)
                            .OrderBy(t => t.CreatedAt).ThenBy(t => t.Id)
                            .Select(t => (Guid?)t.Id)
                            .FirstOrDefault(),
                        r.GuideReview != null),
                    r.GuideProfile!.UserId))
                .FirstOrDefaultAsync(cancellationToken);

        public Task<GuideSessionAccessModel?> GetSessionAsync(Guid sessionId, CancellationToken cancellationToken = default) =>
            _context.Set<GuideSession>()
                .AsNoTracking()
                .Where(s => s.Id == sessionId && !s.IsDeleted)
                .Select(s => new GuideSessionAccessModel(
                    new GuideSessionDto(s.Id, s.GuideRequestId, s.MeetingLocation, s.AgreedPrice, s.Currency),
                    s.GuideRequest.UserId,
                    s.GuideRequest.GuideProfile!.UserId))
                .FirstOrDefaultAsync(cancellationToken);

        public Task<ChatThreadAccessModel?> GetChatThreadAsync(Guid threadId, CancellationToken cancellationToken = default) =>
            _context.Set<ChatThread>()
                .AsNoTracking()
                .Where(t => t.Id == threadId && !t.IsDeleted)
                .Select(t => new ChatThreadAccessModel(
                    t.Id,
                    t.GuideRequestId,
                    t.IsClosed,
                    t.GuideRequest.UserId,
                    t.GuideRequest.GuideProfile!.UserId))
                .FirstOrDefaultAsync(cancellationToken);

        public async Task<PagedResult<ChatMessageDto>> ListMessagesAsync(
            Guid threadId, int page, int pageSize, CancellationToken cancellationToken = default)
        {
            var query = _context.Set<ChatMessage>()
                .AsNoTracking()
                .Where(m => m.ChatThreadId == threadId && !m.IsDeleted);

            var total = await query.CountAsync(cancellationToken);

            // Oldest first: the conversation reads in chronological order; Id is the tie-breaker for a stable page boundary.
            var items = await query
                .OrderBy(m => m.SentAt)
                .ThenBy(m => m.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(m => new ChatMessageDto(m.Id, m.ChatThreadId, m.UserId, m.Text, m.SentAt, m.ReadAt))
                .ToListAsync(cancellationToken);

            return new PagedResult<ChatMessageDto>(items, total, page, pageSize);
        }

        public Task<GuideReviewTarget?> GetReviewTargetAsync(Guid guideProfileId, CancellationToken cancellationToken = default) =>
            _context.Set<GuideProfile>()
                .AsNoTracking()
                .Where(p => p.Id == guideProfileId && !p.IsDeleted)
                .Select(p => new GuideReviewTarget(p.Id, p.UserId, p.Status))
                .FirstOrDefaultAsync(cancellationToken);

        public Task<GuideReviewDto?> GetReviewAsync(Guid reviewId, CancellationToken cancellationToken = default) =>
            _context.Set<GuideReview>()
                .AsNoTracking()
                .Where(r => r.Id == reviewId && !r.IsDeleted)
                .Select(r => new GuideReviewDto(
                    r.Id,
                    r.GuideRequestId,
                    r.GuideProfileId,
                    r.GuideRequest!.User!.FullName,
                    r.Rating,
                    r.Text,
                    r.CreatedAt))
                .FirstOrDefaultAsync(cancellationToken);

        public async Task<PagedResult<GuideReviewDto>> ListReviewsAsync(
            Guid guideProfileId, int page, int pageSize, CancellationToken cancellationToken = default)
        {
            var query = _context.Set<GuideReview>()
                .AsNoTracking()
                .Where(r => r.GuideProfileId == guideProfileId && !r.IsDeleted);

            var total = await query.CountAsync(cancellationToken);

            var items = await query
                .OrderByDescending(r => r.CreatedAt)
                .ThenBy(r => r.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(r => new GuideReviewDto(
                    r.Id,
                    r.GuideRequestId,
                    r.GuideProfileId,
                    r.GuideRequest!.User!.FullName,
                    r.Rating,
                    r.Text,
                    r.CreatedAt))
                .ToListAsync(cancellationToken);

            return new PagedResult<GuideReviewDto>(items, total, page, pageSize);
        }

        private async Task<PagedResult<GuideRequestListItemDto>> ListAsync(
            System.Linq.Expressions.Expression<Func<GuideRequest, bool>> owner,
            RequestStatus? status,
            int page,
            int pageSize,
            CancellationToken cancellationToken)
        {
            var query = _context.Set<GuideRequest>()
                .AsNoTracking()
                .Where(r => !r.IsDeleted)
                .Where(owner);

            if (status is { } filter)
                query = query.Where(r => r.Status == filter);

            var total = await query.CountAsync(cancellationToken);

            var items = await query
                .OrderByDescending(r => r.CreatedAt)
                .ThenBy(r => r.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(r => new GuideRequestListItemDto(
                    r.Id,
                    r.GuideProfileId,
                    r.GuideProfile!.User!.FullName,
                    r.User!.FullName,
                    r.Date,
                    r.StartTime,
                    r.DurationHours,
                    r.PartySize,
                    r.PreferredLanguage,
                    r.Status,
                    r.ExpiresAt,
                    r.CreatedAt))
                .ToListAsync(cancellationToken);

            return new PagedResult<GuideRequestListItemDto>(items, total, page, pageSize);
        }
    }
}
