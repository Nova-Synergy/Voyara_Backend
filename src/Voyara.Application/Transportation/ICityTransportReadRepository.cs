using Voyara.Application.DTOs;
using Voyara.Application.DTOs.Transportation;

namespace Voyara.Application.Transportation;

/// <summary>Optional filters of the transportation listing; a null value means "not filtered".</summary>
/// <param name="CityId">Only records of this city.</param>
/// <param name="TransportTypeId">Only records of this transport type.</param>
/// <param name="DestinationId">Only records that reach this destination.</param>
/// <param name="MinCost">Minimum approximate cost, compared in <paramref name="Currency"/>.</param>
/// <param name="MaxCost">Maximum approximate cost, compared in <paramref name="Currency"/>.</param>
/// <param name="Currency">Upper-case ISO 4217 code the records are stored in.</param>
public sealed record CityTransportSearchCriteria(
    Guid? CityId,
    int? TransportTypeId,
    Guid? DestinationId,
    decimal? MinCost,
    decimal? MaxCost,
    string? Currency);

/// <summary>Read side (CQRS) of the transportation module; returns projections, never entities.</summary>
public interface ICityTransportReadRepository
{
    /// <summary>Returns one filtered page of city transports with deterministic ordering.</summary>
    Task<PagedResult<CityTransportListItemDto>> SearchAsync(
        CityTransportSearchCriteria criteria, int page, int pageSize, CancellationToken cancellationToken = default);

    /// <summary>Returns the full details of a city transport, or <see langword="null"/>.</summary>
    Task<CityTransportDetailsDto?> GetDetailsAsync(Guid id, CancellationToken cancellationToken = default);
}
