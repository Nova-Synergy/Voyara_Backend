namespace Voyara.Application.DTOs;

/// <summary>Read model of a transport route with its stations.</summary>
public sealed record TransportRouteResponse(
    Guid Id,
    string Name,
    string Description,
    decimal? Fare,
    IReadOnlyList<StationResponse> Stations);
