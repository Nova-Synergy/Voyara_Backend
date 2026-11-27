using MediatR;
using Voyara.Application.Abstractions;
using Voyara.Application.DTOs;
using Voyara.Domain.Entities.Catalog;
using Voyara.Domain.Enums;
using Voyara.Domain.Repositories.Catalog;
using Voyara.SharedKernel.Results;

namespace Voyara.Application.Recommendations.Queries.GetDestinationRecommendations;

/// <summary>
/// Handles retrieving recommended destinations using pluggable recommendation strategies with popularity and rating fallbacks.
/// </summary>
public sealed class GetDestinationRecommendationsQueryHandler
    : IRequestHandler<GetDestinationRecommendationsQuery, Result<IReadOnlyList<DestinationRecommendationResponse>>>
{
    private readonly IDestinationRepository _destinationRepository;
    private readonly IRecommendationStrategy _recommendationStrategy;
    private readonly ICurrentUser _currentUser;

    /// <summary>
    /// Initializes a new instance of <see cref="GetDestinationRecommendationsQueryHandler"/>.
    /// </summary>
    public GetDestinationRecommendationsQueryHandler(
        IDestinationRepository destinationRepository,
        IRecommendationStrategy recommendationStrategy,
        ICurrentUser currentUser)
    {
        _destinationRepository = destinationRepository;
        _recommendationStrategy = recommendationStrategy;
        _currentUser = currentUser;
    }

    /// <inheritdoc />
    public async Task<Result<IReadOnlyList<DestinationRecommendationResponse>>> Handle(
        GetDestinationRecommendationsQuery request,
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
                take,
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

        // 2. Fall back / supplement with top popular published destinations
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
            var coverImage = d.Images.FirstOrDefault(i => i.IsCover)?.Url ?? d.Images.FirstOrDefault()?.Url;

            var matchScore = Math.Round(
                Math.Clamp((d.PopularityScore / 100.0 * 0.4) + (d.AverageRating / 5.0 * 0.6), 0.1, 1.0),
                2);

            var reason = d.ReviewCount > 0
                ? $"Top-rated destination ({d.AverageRating:F1}★ from {d.ReviewCount} reviews)"
                : "Popular travel choice based on visitor interest";

            return new DestinationRecommendationResponse(
                d.Id,
                translation.Name,
                translation.Description,
                coverImage,
                d.AverageRating,
                d.ReviewCount,
                d.PopularityScore,
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
