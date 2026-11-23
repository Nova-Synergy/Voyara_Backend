using Voyara.Application.Abstractions.Messaging;
using Voyara.Application.DTOs;

namespace Voyara.Application.Recommendations.Queries.GetDestinationRecommendations;

/// <summary>
/// Query to retrieve top personalized or trending destination recommendations.
/// </summary>
/// <param name="Limit">Maximum number of recommendations to retrieve (defaults to 5).</param>
/// <param name="CityId">Optional city filter.</param>
/// <param name="CategoryId">Optional category filter.</param>
public sealed record GetDestinationRecommendationsQuery(
    int Limit = 5,
    Guid? CityId = null,
    Guid? CategoryId = null) : IQuery<IReadOnlyList<DestinationRecommendationResponse>>;
