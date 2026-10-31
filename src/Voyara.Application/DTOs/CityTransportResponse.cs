namespace Voyara.Application.DTOs;

/// <summary>Read model of a city transport with its routes and linked destinations.</summary>
public sealed record CityTransportResponse(
    Guid Id,
    int CityId,
    int TransportTypeId,
    string Description,
    decimal ApproxCost,
    string Currency,
    string OperatingHours,
    string SafetyInfo,
    DateTimeOffset? UpdatedAt,
    IReadOnlyList<TransportRouteResponse> Routes,
    IReadOnlyList<Guid> DestinationIds);
