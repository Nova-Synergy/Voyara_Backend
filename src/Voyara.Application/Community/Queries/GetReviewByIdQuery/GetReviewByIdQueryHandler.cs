using MediatR;
using Voyara.Application.Abstractions;
using Voyara.Application.Abstractions.Authorization;
using Voyara.Application.Community.DTOs;
using Voyara.Domain.Enums;
using Voyara.Domain.Repositories.Community;
using Voyara.SharedKernel.Results;

namespace Voyara.Application.Community.Queries.GetReviewById;

/// <summary>
/// Handles the GetReviewByIdQuery, retrieving a review by its unique identifier and returning it
/// in a ReviewResponse DTO wrapped in a Result.</summary>
/// <remarks>
/// <para>
/// Uses the IReviewRepository to fetch the review by Id, and returns a NotFound error if the
/// review is not found.</para>
/// <para>
/// Checks if the current user is the review owner or an admin; if not, and the review is not
/// Approved, returns a NotFound error.</para>
/// <para>
/// Otherwise, returns the review data mapped to a ReviewResponse DTO via the From helper
/// method.</para>
/// </remarks>
[AuthorizeRequest(Roles.Tourist)]
public sealed class GetReviewByIdQueryHandler
    : IRequestHandler<GetReviewByIdQuery, Result<ReviewResponse>>
{
    private readonly IReviewRepository _repository;
    private readonly ICurrentUser _currentUser;

    public GetReviewByIdQueryHandler(
        IReviewRepository repository,
        ICurrentUser currentUser)
    {
        _repository = repository;
        _currentUser = currentUser;
    }

    public async Task<Result<ReviewResponse>> Handle(
        GetReviewByIdQuery request,
        CancellationToken cancellationToken)
    {
        var review = await _repository.GetByIdAsync(
            request.ReviewId,
            cancellationToken);

        if (review is null)
        {
            return Result.Failure<ReviewResponse>(
                Error.NotFound("Review.NotFound", "Review was not found."));
        }

        var isOwner = _currentUser.UserId == review.UserId;
        var isAdmin = _currentUser.IsInRole(Roles.Admin);

        if (review.Status != ReviewStatus.Approved && !isOwner && !isAdmin)
        {
            return Result.Failure<ReviewResponse>(
                Error.NotFound("Review.NotFound", "Review was not found."));
        }

        return Result.Success(ReviewResponse.From(review));
    }
}