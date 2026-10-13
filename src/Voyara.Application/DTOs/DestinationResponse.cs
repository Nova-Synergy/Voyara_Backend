namespace Voyara.Application.DTOs;

/// <summary>Read model of a destination returned by catalog queries.</summary>
public sealed record DestinationResponse(
    Guid Id,
    string Name,
    string? Description,
    double Latitude,
    double Longitude,
    string Status);
