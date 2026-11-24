namespace Voyara.Application.DTOs;

/// <summary>
/// Read model of a destination shown in search results. The first six members are those of the former
/// <see cref="DestinationResponse"/> returned by this endpoint, so existing consumers keep working.
/// </summary>
public sealed record DestinationSummaryResponse(
    Guid Id,
    string Name,
    string? Description,
    double Latitude,
    double Longitude,
    string Status,
    int CityId,
    Guid CategoryId,
    double AverageRating,
    int ReviewCount,
    int PopularityScore,
    string? CoverImageUrl);
