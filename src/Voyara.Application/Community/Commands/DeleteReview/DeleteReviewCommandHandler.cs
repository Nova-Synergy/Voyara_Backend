using MediatR;
using Voyara.Application.Abstractions;
using Voyara.Domain.Repositories.Community;
using Voyara.SharedKernel.Results;

namespace Voyara.Application.Community.Commands.DeleteReview;

/// <summary>
/// Handles the DeleteReviewCommand, deleting an existing review for a destination and persisting the changes.</summary>
/// <remarks>
/// <para>
/// Requires authorization; if the current user is not authenticated, returns an unauthorized result.</para>
/// <para>
/// Uses the IReviewRepository to fetch the review by Id and the current user Id, then calls
/// Review.Delete to mark the review as deleted and update its soft-delete flag.</para>
/// <para>
/// Commits changes to the database and returns a successful Result if the deletion was valid.</para>
/// </remarks>
public sealed class DeleteReviewCommandHandler
    : IRequestHandler<DeleteReviewCommand, Result>
{
    private readonly IReviewRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUser _currentUser;

    public DeleteReviewCommandHandler(
        IReviewRepository repository,
        IUnitOfWork unitOfWork,
        ICurrentUser currentUser)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }

    public async Task<Result> Handle(
        DeleteReviewCommand request,
        CancellationToken cancellationToken)
    {
        if (_currentUser.UserId is not Guid userId)
        {
            return Result.Failure(
                Error.Unauthorized("Review.Unauthenticated", "Authentication is required."));
        }

        var review = await _repository.GetByIdForUserAsync(
            request.ReviewId,
            userId,
            cancellationToken);

        if (review is null)
        {
            return Result.Failure(
                Error.NotFound("Review.NotFound", "Review was not found."));
        }

        var result = review.Delete(userId);

        if (result.IsFailure)
            return result;

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}