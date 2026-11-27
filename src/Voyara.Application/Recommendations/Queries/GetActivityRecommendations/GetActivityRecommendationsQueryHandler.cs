using MediatR;
using Voyara.Application.Abstractions;
using Voyara.Application.DTOs;
using Voyara.Domain.Entities.Catalog;
using Voyara.Domain.Enums;
using Voyara.Domain.Repositories.Catalog;
using Voyara.SharedKernel.Results;

namespace Voyara.Application.Recommendations.Queries.GetActivityRecommendations;

/// <summary>
/// Handles retrieving recommended activities and destination attractions using pluggable strategies with popularity fallbacks.
/// </summary>
public sealed class GetActivityRecommendationsQueryHandler
    : IRequestHandler<GetActivityRecommendationsQuery, Result<IReadOnlyList<ActivityRecommendationResponse>>>
{
    private readonly IDestinationRepository _destinationRepository;
    private readonly IRecommendationStrategy _recommendationStrategy;
    private readonly ICurrentUser _currentUser;

    /// <summary>
    /// Initializes a new instance of <see cref="GetActivityRecommendationsQueryHandler"/>.
    /// </summary>
    public GetActivityRecommendationsQueryHandler(
        IDestinationRepository destinationRepository,
        IRecommendationStrategy recommendationStrategy,
        ICurrentUser currentUser)
    {
        _destinationRepository = destinationRepository;
        _recommendationStrategy = recommendationStrategy;
        _currentUser = currentUser;
    }

    /// <inheritdoc />
    public async Task<Result<IReadOnlyList<ActivityRecommendationResponse>>> Handle(
        GetActivityRecommendationsQuery request,
        CancellationToken cancellationToken)
    {
        var take = Math.Clamp(request.Limit, 1, 50);
        var collected = new List<Destination>();
        var seenIds = new HashSet<Guid>();

        // 1. Try strategy recommendations if user is authenticated
        if (_currentUser.UserId.HasValue)
        {
            var recommendedIds = await _recommendationStrategy.RecommendAsync(
                _currentUser.UserId.Value,
                take * 2,
                cancellationToken);

            foreach (var id in recommendedIds)
            {
                if (seenIds.Add(id))
                {
                    var dest = await _destinationRepository.GetByIdAsync(id, cancellationToken);
                    if (dest is not null && MatchesFilter(dest, request.CityId, request.CategoryId))
                    {
                        collected.Add(dest);
                    }
                }
            }
        }

        // 2. Supplement / fallback with top popular published attractions
        if (collected.Count < take)
        {
            var candidates = await _destinationRepository.SearchAsync(
                searchTerm: null,
                status: DestinationStatus.Published,
                skip: 0,
                take: take * 3,
                cancellationToken: cancellationToken);

            var ranked = candidates
                .Where(d => MatchesFilter(d, request.CityId, request.CategoryId))
                .Where(d => seenIds.Add(d.Id))
                .OrderByDescending(d => d.PopularityScore)
                .ThenByDescending(d => d.AverageRating)
                .Take(take - collected.Count);

            collected.AddRange(ranked);
        }

        var responses = collected.Select(d =>
        {
            var translation = d.GetTranslation();
            var ticket = d.TicketPrices.FirstOrDefault(tp => tp.VisitorType == VisitorType.Adult)
                         ?? d.TicketPrices.FirstOrDefault();

            var admissionPrice = ticket?.Price.Amount ?? 0m;
            var currency = ticket?.Price.Currency ?? "USD";

            var matchScore = Math.Round(
                Math.Clamp((d.PopularityScore / 100.0 * 0.4) + (d.AverageRating / 5.0 * 0.6), 0.1, 1.0),
                2);

            var reason = d.EstimatedVisitMinutes > 0
                ? $"Top-rated activity ({d.EstimatedVisitMinutes} mins, {d.AverageRating:F1}★ from {d.ReviewCount} reviews)"
                : $"Popular attraction ({d.AverageRating:F1}★ from {d.ReviewCount} reviews)";

            var category = d.Category?.Code ?? "Attraction";

            return new ActivityRecommendationResponse(
                d.Id,
                translation.Name,
                translation.Description,
                category,
                admissionPrice,
                currency,
                d.EstimatedVisitMinutes,
                matchScore,
                reason);
        }).ToList();

        return responses;
    }

    private static bool MatchesFilter(Destination d, Guid? cityId, Guid? categoryId)
    {
        if (cityId.HasValue && d.CityId != cityId.Value)
        {
            return false;
        }

        if (categoryId.HasValue && d.CategoryId != categoryId.Value)
        {
            return false;
        }

        return true;
    }
}
