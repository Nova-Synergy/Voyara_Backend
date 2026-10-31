using Voyara.Application.DTOs;
using Voyara.Domain.Entities.Transportation;

namespace Voyara.Application.Transportation;

/// <summary>Maps Transportation aggregates to read models.</summary>
internal static class CityTransportMappings
{
    /// <summary>Maps a city transport with its routes, stations and destinations.</summary>
    public static CityTransportResponse ToResponse(this CityTransport cityTransport) => new(
        cityTransport.Id,
        cityTransport.CityId,
        cityTransport.TransportTypeId,
        cityTransport.Description,
        cityTransport.ApproxCost,
        cityTransport.Currency,
        cityTransport.OperatingHours,
        cityTransport.SafetyInfo,
        cityTransport.UpdatedAt,
        cityTransport.Routes.Select(route => route.ToResponse()).ToList(),
        cityTransport.Destinations.Select(link => link.DestinationId).ToList());

    /// <summary>Maps a city transport to its short list model.</summary>
    public static CityTransportSummaryResponse ToSummary(this CityTransport cityTransport) => new(
        cityTransport.Id,
        cityTransport.CityId,
        cityTransport.TransportTypeId,
        cityTransport.Description,
        cityTransport.ApproxCost,
        cityTransport.Currency,
        cityTransport.OperatingHours,
        cityTransport.Routes.Count);

    private static TransportRouteResponse ToResponse(this TransportRoute route) => new(
        route.Id,
        route.Name,
        route.Description,
        route.Fare,
        route.Stations.Select(station => station.ToResponse()).ToList());

    private static StationResponse ToResponse(this Station station) => new(
        station.Id,
        station.Name,
        station.Location.Latitude,
        station.Location.Longitude);
}
