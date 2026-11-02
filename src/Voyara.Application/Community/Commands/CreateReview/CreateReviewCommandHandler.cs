using MediatR;
using Voyara.Application.Abstractions;
using Voyara.Application.Abstractions.Authorization;
using Voyara.Domain.Entities.Community;
using Voyara.Domain.Repositories.Community;
using Voyara.SharedKernel.Results;

namespace Voyara.Application.Community.Commands.CreateReview;

/// <summary>
/// Handles the CreateReviewCommand, creating a new review for a destination and persisting it.
/// </summary>
/// <remarks>
/// <para>
/// Requires authorization; if the current user is not authenticated, returns an unauthorized result.
/// </para>
/// <para>
/// Uses Review.Create to validate the review data and then persists it via the IReviewRepository.
/// </para>
/// <para>
/// Commits changes to the database and returns the created review's Id wrapped in a successful Result.
/// </para>
/// </remarks>
[AuthorizeRequest(Roles.Tourist)]
public sealed class CreateReviewCommandHandler
    : IRequestHandler<CreateReviewCommand, Result<Guid>>
{
    private readonly IReviewRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUser _currentUser;

    public CreateReviewCommandHandler(
        IReviewRepository repository,
        IUnitOfWork unitOfWork,
        ICurrentUser currentUser)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }

    public async Task<Result<Guid>> Handle(
        CreateReviewCommand request,
        CancellationToken cancellationToken)
    {
        if (_currentUser.UserId is not Guid userId)
        {
            return Result.Failure<Guid>(
                Error.Unauthorized("Review.Unauthenticated", "Authentication is required."));
        }

        var result = Review.Create(
            userId,
            request.DestinationId,
            request.Rating,
            request.Text,
            request.VisitDate);

        if (result.IsFailure)
            return Result.Failure<Guid>(result.Error);

        _repository.Add(result.Value);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(result.Value.Id);
    }
}