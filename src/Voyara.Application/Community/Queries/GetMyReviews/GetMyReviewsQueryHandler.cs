using MediatR;
using Voyara.Application.Abstractions;
using Voyara.Application.Community.DTOs;
using Voyara.Domain.Repositories.Community;
using Voyara.SharedKernel.Results;

namespace Voyara.Application.Community.Queries.GetMyReviews;

/// <summary>
/// Handles the GetMyReviewsQuery, retrieving a list of reviews submitted by the currently authenticated user.</summary>
/// <remarks>
/// <para>
/// Uses the IReviewRepository to fetch reviews by the user's Id, with pagination support.</para>
/// <para>
/// Validates the page and page size parameters, clamping page size to 1-100.</para>
/// <para>
/// Maps each review to a ReviewResponse DTO using the From helper method and returns a
/// successful Result containing the list of DTOs.</para>
/// </remarks>
public sealed class GetMyReviewsQueryHandler
    : IRequestHandler<GetMyReviewsQuery, Result<IReadOnlyList<ReviewResponse>>>
{
    private readonly IReviewRepository _repository;
    private readonly ICurrentUser _currentUser;

    public GetMyReviewsQueryHandler(
        IReviewRepository repository,
        ICurrentUser currentUser)
    {
        _repository = repository;
        _currentUser = currentUser;
    }

    public async Task<Result<IReadOnlyList<ReviewResponse>>> Handle(
        GetMyReviewsQuery request,
        CancellationToken cancellationToken)
    {
        if (_currentUser.UserId is not Guid userId)
        {
            return Result.Failure<IReadOnlyList<ReviewResponse>>(
                Error.Unauthorized("Review.Unauthenticated", "Authentication is required."));
        }

        var page = Math.Max(1, request.Page);
        var pageSize = Math.Clamp(request.PageSize, 1, 100);
        var skip = (page - 1) * pageSize;

        var reviews = await _repository.GetByUserAsync(
            userId,
            skip,
            pageSize,
            cancellationToken);

        IReadOnlyList<ReviewResponse> response = reviews
            .Select(ReviewResponse.From)
            .ToList();

        return Result.Success(response);
    }
}