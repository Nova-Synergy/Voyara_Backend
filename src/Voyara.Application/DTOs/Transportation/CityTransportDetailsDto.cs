namespace Voyara.Application.DTOs.Transportation;

/// <summary>Full details of a city transport (FR-TRN-02).</summary>
/// <param name="Id">Identifier of the record.</param>
/// <param name="CityId">Identifier of the city.</param>
/// <param name="TransportTypeId">Identifier of the transport type.</param>
/// <param name="TransportTypeCode">Machine-readable transport type code.</param>
/// <param name="TransportTypeName">Display name of the transport type.</param>
/// <param name="Description">General description.</param>
/// <param name="ApproxCost">Approximate cost in <paramref name="Currency"/> (an estimate).</param>
/// <param name="Currency">ISO 4217 currency code of the cost.</param>
/// <param name="OperatingHours">Free-text operating hours.</param>
/// <param name="SafetyInfo">Safety tips; may be empty.</param>
/// <param name="LastUpdatedAt">Last update time (creation time if never updated).</param>
/// <param name="Routes">Routes with their stations.</param>
/// <param name="DestinationIds">Destinations reached by this transport.</param>
public sealed record CityTransportDetailsDto(
    Guid Id,
    Guid CityId,
    int TransportTypeId,
    string TransportTypeCode,
    string TransportTypeName,
    string Description,
    decimal ApproxCost,
    string Currency,
    string OperatingHours,
    string SafetyInfo,
    DateTimeOffset? LastUpdatedAt,
    IReadOnlyList<TransportRouteDto> Routes,
    IReadOnlyList<Guid> DestinationIds);

/// <summary>A route of a city transport.</summary>
/// <param name="Id">Identifier of the route.</param>
/// <param name="Name">Route name.</param>
/// <param name="Description">Route description.</param>
/// <param name="Fare">Fare for the whole route, or null when unknown.</param>
/// <param name="Stations">Stations on the route.</param>
public sealed record TransportRouteDto(
    Guid Id,
    string Name,
    string Description,
    decimal? Fare,
    IReadOnlyList<StationDto> Stations);

/// <summary>A station on a route.</summary>
/// <param name="Id">Identifier of the station.</param>
/// <param name="Name">Station name.</param>
/// <param name="Latitude">Latitude in degrees.</param>
/// <param name="Longitude">Longitude in degrees.</param>
public sealed record StationDto(Guid Id, string Name, double Latitude, double Longitude);
