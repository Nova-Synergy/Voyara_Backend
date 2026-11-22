using Voyara.Application.DTOs;
using Voyara.Application.DTOs.Guides;
using Voyara.Domain.Enums;

namespace Voyara.Application.Guides;

/// <summary>A weekly availability window of a guide.</summary>
public sealed record GuideAvailabilityWindow(DayOfWeek Day, TimeOnly StartTime, TimeOnly EndTime);

/// <summary>The data needed to decide whether a guide can receive a request.</summary>
public sealed record GuideBookingContext(
    Guid GuideProfileId,
    Guid GuideUserId,
    GuideStatus Status,
    IReadOnlyList<GuideAvailabilityWindow> Availability,
    IReadOnlyList<DateOnly> TimeOffDates);

/// <summary>A request projection together with its two participants, used for ownership checks.</summary>
public sealed record GuideRequestAccessModel(GuideRequestDetailsDto Request, Guid GuideUserId);

/// <summary>A session projection together with its two participants.</summary>
public sealed record GuideSessionAccessModel(GuideSessionDto Session, Guid TouristUserId, Guid GuideUserId);

/// <summary>A chat thread projection together with its two participants.</summary>
public sealed record ChatThreadAccessModel(
    Guid Id, Guid GuideRequestId, bool IsClosed, Guid TouristUserId, Guid GuideUserId);

/// <summary>Public header of a guide, used to decide review visibility.</summary>
public sealed record GuideReviewTarget(Guid GuideProfileId, Guid GuideUserId, GuideStatus Status);

/// <summary>Read-side (query) access for guide requests, sessions, chat and reviews. Returns DTOs only.</summary>
public interface IGuideRequestReadRepository
{
    Task<GuideBookingContext?> GetBookingContextAsync(Guid guideProfileId, CancellationToken cancellationToken = default);

    Task<PagedResult<GuideRequestListItemDto>> ListForTouristAsync(
        Guid touristUserId, RequestStatus? status, int page, int pageSize, CancellationToken cancellationToken = default);

    Task<PagedResult<GuideRequestListItemDto>> ListForGuideAsync(
        Guid guideUserId, RequestStatus? status, int page, int pageSize, CancellationToken cancellationToken = default);

    Task<GuideRequestAccessModel?> GetRequestAsync(Guid requestId, CancellationToken cancellationToken = default);

    Task<GuideSessionAccessModel?> GetSessionAsync(Guid sessionId, CancellationToken cancellationToken = default);

    Task<ChatThreadAccessModel?> GetChatThreadAsync(Guid threadId, CancellationToken cancellationToken = default);

    Task<PagedResult<ChatMessageDto>> ListMessagesAsync(
        Guid threadId, int page, int pageSize, CancellationToken cancellationToken = default);

    Task<GuideReviewTarget?> GetReviewTargetAsync(Guid guideProfileId, CancellationToken cancellationToken = default);

    Task<GuideReviewDto?> GetReviewAsync(Guid reviewId, CancellationToken cancellationToken = default);

    Task<PagedResult<GuideReviewDto>> ListReviewsAsync(
        Guid guideProfileId, int page, int pageSize, CancellationToken cancellationToken = default);
}
