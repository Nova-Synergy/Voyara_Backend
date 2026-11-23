using MediatR;
using Voyara.Application.Abstractions;
using Voyara.Application.DTOs;
using Voyara.Domain.Entities.TripAndJourney;
using Voyara.Domain.Repositories.TripAndJourney;
using Voyara.SharedKernel.Results;

namespace Voyara.Application.Recommendations.Queries.GetJourneyRecommendations;

/// <summary>
/// Handles retrieving recommended journeys based on user preferences, duration filters, and AI synthesis.
/// </summary>
public sealed class GetJourneyRecommendationsQueryHandler
    : IRequestHandler<GetJourneyRecommendationsQuery, Result<IReadOnlyList<JourneyRecommendationResponse>>>
{
    private readonly IJourneyRepository _journeyRepository;
    private readonly IRecommendationStrategy _recommendationStrategy;
    private readonly ICurrentUser _currentUser;

    /// <summary>
    /// Initializes a new instance of <see cref="GetJourneyRecommendationsQueryHandler"/>.
    /// </summary>
    public GetJourneyRecommendationsQueryHandler(
        IJourneyRepository journeyRepository,
        IRecommendationStrategy recommendationStrategy,
        ICurrentUser currentUser)
    {
        _journeyRepository = journeyRepository;
        _recommendationStrategy = recommendationStrategy;
        _currentUser = currentUser;
    }

    /// <inheritdoc />
    public async Task<Result<IReadOnlyList<JourneyRecommendationResponse>>> Handle(
        GetJourneyRecommendationsQuery request,
        CancellationToken cancellationToken)
    {
        var take = Math.Clamp(request.Limit, 1, 50);

        var candidateJourneys = await _journeyRepository.GetRecommendedCandidatesAsync(
            request.TotalDays,
            take * 3,
            cancellationToken);

        // Fetch user preferred destinations if authenticated to evaluate overlap
        var preferredDestinationIds = new HashSet<Guid>();
        if (_currentUser.UserId.HasValue)
        {
            var recommendedDestIds = await _recommendationStrategy.RecommendAsync(
                _currentUser.UserId.Value,
                take * 3,
                cancellationToken);

            foreach (var id in recommendedDestIds)
            {
                preferredDestinationIds.Add(id);
            }
        }

        var scoredJourneys = candidateJourneys.Select(journey =>
        {
            var allItems = journey.Days.SelectMany(d => d.Items).ToList();
            var totalStops = allItems.Count;
            var highlights = allItems.Select(i => i.DestinationId).Distinct().ToList();

            // Calculate affinity / match score (0.1 to 1.0)
            var score = 0.5;

            var matchingStops = highlights.Count(h => preferredDestinationIds.Contains(h));
            if (matchingStops > 0)
            {
                score += Math.Min(0.3, matchingStops * 0.1);
            }

            if (request.TotalDays.HasValue && journey.TotalDays == request.TotalDays.Value)
            {
                score += 0.15;
            }

            if (journey.GeneratedByAI)
            {
                score += 0.1;
            }

            score = Math.Round(Math.Clamp(score, 0.1, 1.0), 2);

            string reason;
            if (matchingStops > 0)
            {
                reason = $"Curated {journey.TotalDays}-day itinerary matching {matchingStops} of your preferred destinations";
            }
            else if (journey.GeneratedByAI)
            {
                reason = $"AI-optimized {journey.TotalDays}-day itinerary with {totalStops} curated stops";
            }
            else
            {
                reason = $"Featured {journey.TotalDays}-day travel itinerary with {totalStops} stops";
            }

            return new JourneyRecommendationResponse(
                journey.Id,
                journey.Title,
                journey.TotalDays,
                totalStops,
                journey.GeneratedByAI,
                journey.Status.ToString(),
                score,
                reason,
                highlights);
        })
        .OrderByDescending(j => j.MatchScore)
        .ThenByDescending(j => j.TotalStops)
        .Take(take)
        .ToList();

        return scoredJourneys;
    }
}
