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

/// <summary>
/// Read model representing a recommended journey itinerary with affinity scoring, highlights, and explainability rationale.
/// </summary>
/// <param name="JourneyId">The journey's unique identifier.</param>
/// <param name="Title">Descriptive title of the itinerary.</param>
/// <param name="TotalDays">Total number of days in the itinerary.</param>
/// <param name="TotalStops">Total number of stops across all days.</param>
/// <param name="GeneratedByAI">Whether the journey was synthesized by the AI assistant.</param>
/// <param name="Status">Current planning status of the journey.</param>
/// <param name="MatchScore">Normalized affinity match score (0.0 to 1.0).</param>
/// <param name="Reason">AI or algorithmic justification for the recommendation.</param>
/// <param name="Highlights">List of destination identifiers featured as stops in the journey.</param>
public sealed record JourneyRecommendationResponse(
    Guid JourneyId,
    string Title,
    int TotalDays,
    int TotalStops,
    bool GeneratedByAI,
    string Status,
    double MatchScore,
    string Reason,
    IReadOnlyList<Guid> Highlights);

/// <summary>
/// Read model representing a recommended activity or destination attraction with affinity scoring and pricing details.
/// </summary>
/// <param name="DestinationId">The destination's unique identifier.</param>
/// <param name="Name">Localized name of the activity or destination.</param>
/// <param name="Description">Localized description or highlights.</param>
/// <param name="Category">Category code or classification.</param>
/// <param name="AdmissionPrice">Base ticket admission price, if applicable.</param>
/// <param name="Currency">Currency code of the admission price (e.g. USD).</param>
/// <param name="EstimatedVisitMinutes">Estimated duration of the activity in minutes.</param>
/// <param name="MatchScore">Normalized affinity match score (0.0 to 1.0).</param>
/// <param name="Reason">AI or algorithmic justification for the recommendation.</param>
public sealed record ActivityRecommendationResponse(
    Guid DestinationId,
    string Name,
    string? Description,
    string? Category,
    decimal AdmissionPrice,
    string Currency,
    int EstimatedVisitMinutes,
    double MatchScore,
    string Reason);

/// <summary>
/// Payload to submit tourist feedback or ratings on a recommended destination.
/// </summary>
/// <param name="DestinationId">The recommended destination identifier.</param>
/// <param name="Score">Affinity rating or satisfaction score (0.0 to 5.0).</param>
/// <param name="FeedbackReason">Optional feedback rationale or comments.</param>
public sealed record SubmitRecommendationFeedbackRequest(
    Guid DestinationId,
    double Score,
    string? FeedbackReason);

/// <summary>
/// Read model representing the recorded recommendation feedback audit entry.
/// </summary>
/// <param name="FeedbackId">The unique feedback log record identifier.</param>
/// <param name="DestinationId">The recommended destination identifier.</param>
/// <param name="UserId">The user identifier who submitted feedback.</param>
/// <param name="Score">Recorded affinity or satisfaction score.</param>
/// <param name="FeedbackReason">Recorded rationale or comments.</param>
/// <param name="CreatedAt">Timestamp when feedback was submitted.</param>
public sealed record RecommendationFeedbackResponse(
    long FeedbackId,
    Guid DestinationId,
    Guid UserId,
    double Score,
    string? FeedbackReason,
    DateTime CreatedAt);
