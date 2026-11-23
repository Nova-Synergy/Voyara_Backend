using Voyara.Application.Abstractions.Messaging;
using Voyara.Application.DTOs;
using Voyara.Application.DTOs.Transportation;

namespace Voyara.Application.Transportation.Queries.GetCityTransports;

/// <summary>
/// Paginated, filterable listing of city transports. The cost range applies to one currency,
/// so <see cref="Currency"/> is required when a cost bound is given.
/// </summary>
public sealed record GetCityTransportsQuery(
    Guid? CityId,
    int? TransportTypeId,
    Guid? DestinationId,
    decimal? MinCost,
    decimal? MaxCost,
    string? Currency,
    int Page = 1,
    int PageSize = 10) : IQuery<PagedResult<CityTransportListItemDto>>;
