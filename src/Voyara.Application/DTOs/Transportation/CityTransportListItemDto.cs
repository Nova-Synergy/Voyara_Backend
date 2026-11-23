namespace Voyara.Application.DTOs.Transportation;

/// <summary>Lightweight listing row of a city transport.</summary>
/// <param name="Id">Identifier of the record.</param>
/// <param name="CityId">Identifier of the city.</param>
/// <param name="TransportTypeId">Identifier of the transport type.</param>
/// <param name="TransportTypeCode">Machine-readable transport type code.</param>
/// <param name="TransportTypeName">Display name of the transport type.</param>
/// <param name="Description">General description.</param>
/// <param name="ApproxCost">Approximate cost in <paramref name="Currency"/> (an estimate).</param>
/// <param name="Currency">ISO 4217 currency code of the cost.</param>
/// <param name="OperatingHours">Free-text operating hours.</param>
/// <param name="RouteCount">Number of routes.</param>
/// <param name="LastUpdatedAt">Last update time (creation time if never updated).</param>
public sealed record CityTransportListItemDto(
    Guid Id,
    Guid CityId,
    int TransportTypeId,
    string TransportTypeCode,
    string TransportTypeName,
    string Description,
    decimal ApproxCost,
    string Currency,
    string OperatingHours,
    int RouteCount,
    DateTimeOffset? LastUpdatedAt);
