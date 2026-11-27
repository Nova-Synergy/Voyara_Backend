using Voyara.Application.Abstractions.Messaging;
using Voyara.Application.DTOs;

namespace Voyara.Application.Recommendations.Queries.GetJourneyRecommendations;

/// <summary>
/// Query to retrieve top personalized or trending journey recommendations.
/// </summary>
/// <param name="Limit">Maximum number of recommendations to retrieve (defaults to 5).</param>
/// <param name="TotalDays">Optional planned duration in days filter.</param>
public sealed record GetJourneyRecommendationsQuery(
    int Limit = 5,
    int? TotalDays = null) : IQuery<IReadOnlyList<JourneyRecommendationResponse>>;
