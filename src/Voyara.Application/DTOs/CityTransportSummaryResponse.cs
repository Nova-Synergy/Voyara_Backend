namespace Voyara.Application.DTOs;

/// <summary>Short read model of a city transport, used in lists.</summary>
public sealed record CityTransportSummaryResponse(
    Guid Id,
    int CityId,
    int TransportTypeId,
    string Description,
    decimal ApproxCost,
    string Currency,
    string OperatingHours,
    int RouteCount);
