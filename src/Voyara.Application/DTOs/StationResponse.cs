namespace Voyara.Application.DTOs;

/// <summary>Read model of a station on a transport route.</summary>
public sealed record StationResponse(Guid Id, string Name, double Latitude, double Longitude);
