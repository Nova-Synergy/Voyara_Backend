namespace Voyara.Application.DTOs;

/// <summary>Compact read model of a destination shown in search results.</summary>
public sealed record DestinationSummaryResponse(
    Guid Id,
    string Name,
    string? ShortDescription,
    int CityId,
    Guid CategoryId,
    double Latitude,
    double Longitude,
    double AverageRating,
    int ReviewCount,
    int PopularityScore,
    string? CoverImageUrl);
