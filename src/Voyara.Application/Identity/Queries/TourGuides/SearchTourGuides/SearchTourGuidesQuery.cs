using Voyara.Application.Abstractions.Messaging;
using Voyara.Application.DTOs;
using Voyara.Application.DTOs.Identity;
using Voyara.Domain.Enums;

namespace Voyara.Application.Identity.Queries.TourGuides.SearchTourGuides;

/// <summary>
/// Lists guides with paging. The public sees approved guides;
/// an administrator can filter by status.
/// </summary>
public sealed record SearchTourGuidesQuery(
    int? CityId,
    GuideStatus? Status,
    int Page,
    int PageSize) : IQuery<PagedResult<TourGuideReadDto>>;
