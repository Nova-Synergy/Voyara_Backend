using Voyara.Application.DTOs;
using Voyara.Application.DTOs.Guides;
using Voyara.Domain.Enums;

namespace Voyara.Application.Guides;

/// <summary>Filters of the public guide directory (FR-GDE-05).</summary>
public sealed record GuideSearchCriteria(
    Guid? CityId,
    string? LanguageCode,
    int? ExpertiseCategoryId,
    decimal? MinRating,
    decimal? MinPrice,
    decimal? MaxPrice,
    PricingUnit? PricingUnit,
    string? Currency);

/// <summary>Details read model; carries what the handler needs to apply visibility rules.</summary>
public sealed record GuideDetailsReadModel(GuideProfileDetailsDto Profile, Guid UserId, string? RejectionReason);

/// <summary>Read side (CQRS) of the guide module; returns projections, never entities.</summary>
public interface IGuideReadRepository
{
    Task<PagedResult<GuideListItemDto>> SearchApprovedAsync(
        GuideSearchCriteria criteria, int page, int pageSize, CancellationToken cancellationToken = default);

    Task<PagedResult<GuideApplicationListItemDto>> ListApplicationsAsync(
        GuideStatus? status, int page, int pageSize, CancellationToken cancellationToken = default);

    Task<GuideDetailsReadModel?> GetDetailsAsync(Guid profileId, CancellationToken cancellationToken = default);

    Task<GuideDetailsReadModel?> GetDetailsByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<GuideDocumentDto>> GetDocumentsAsync(Guid profileId, CancellationToken cancellationToken = default);
}
