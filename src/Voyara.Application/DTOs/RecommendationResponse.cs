namespace Voyara.Application.DTOs;

/// <summary>
/// Read model representing a recommended destination with affinity scoring and explainability rationale.
/// </summary>
/// <param name="DestinationId">The destination's unique identifier.</param>
/// <param name="Name">Localized name of the destination.</param>
/// <param name="Description">Localized summary or highlights.</param>
/// <param name="CoverImageUrl">Cover photo URL, if available.</param>
/// <param name="Rating">Average visitor rating (0..5).</param>
/// <param name="ReviewCount">Total number of visitor reviews.</param>
/// <param name="PopularityScore">Algorithmic popularity score.</param>
/// <param name="MatchScore">Normalized affinity match score (0.0 to 1.0).</param>
/// <param name="Reason">AI or algorithmic justification for the recommendation.</param>
public sealed record DestinationRecommendationResponse(
    Guid DestinationId,
    string Name,
    string? Description,
    string? CoverImageUrl,
    double Rating,
    int ReviewCount,
    int PopularityScore,
    double MatchScore,
    string Reason);
