using Voyara.Application.Abstractions.Messaging;
using Voyara.Application.DTOs;
using Voyara.Application.DTOs.Guide;
using Voyara.Domain.Enums;

namespace Voyara.Application.Guides.Queries.GetApprovedGuides;

/// <summary>
/// Public directory of approved guides. The price filter applies to one pricing unit and currency
/// (defaults: PerHour, USD) because a guide may offer several prices.
/// </summary>
public sealed record GetApprovedGuidesQuery(
    Guid? CityId,
    string? LanguageCode,
    int? ExpertiseCategoryId,
    decimal? MinRating,
    decimal? MinPrice,
    decimal? MaxPrice,
    PricingUnit? PricingUnit,
    string? Currency,
    int Page = 1,
    int PageSize = 10) : IQuery<PagedResult<GuideListItemDto>>;
