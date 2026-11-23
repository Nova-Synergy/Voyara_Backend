using MediatR;
using Voyara.Application.Abstractions;
using Voyara.Domain.Repositories.Community;
using Voyara.SharedKernel.Results;

namespace Voyara.Application.Community.Commands.DeletePlaceSuggestion;

/// <summary>
/// Handles soft-deleting an existing place suggestion owned by the currently authenticated user.
/// </summary>
public sealed class DeletePlaceSuggestionCommandHandler
    : IRequestHandler<DeletePlaceSuggestionCommand, Result>
{
    private readonly IPlaceSuggestionRepository _repository;
    private readonly ICurrentUser _currentUser;
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>
    /// Initializes a new instance of the <see cref="DeletePlaceSuggestionCommandHandler"/> class.
    /// </summary>
    /// <param name="repository">The repository used to query and persist place suggestion aggregates.</param>
    /// <param name="currentUser">The service providing information about the currently authenticated user.</param>
    /// <param name="unitOfWork">The unit of work used to commit changes to the database.</param>
    public DeletePlaceSuggestionCommandHandler(
        IPlaceSuggestionRepository repository,
        ICurrentUser currentUser,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _currentUser = currentUser;
        _unitOfWork = unitOfWork;
    }

    /// <summary>
    /// Processes the <see cref="DeletePlaceSuggestionCommand"/> to verify ownership,
    /// soft-delete the <see cref="Voyara.Domain.Entities.Community.PlaceSuggestion"/> aggregate, and persist changes.
    /// </summary>
    /// <param name="request">The command containing the identifier of the place suggestion to delete.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// A <see cref="Result"/> indicating success if the suggestion was deleted;
    /// otherwise, a failure result with an unauthorized, not-found, or domain error.
    /// </returns>
    public async Task<Result> Handle(
        DeletePlaceSuggestionCommand request,
        CancellationToken cancellationToken)
    {
        if (_currentUser.UserId is not Guid userId)
        {
            return Result.Failure(
                Error.Unauthorized(
                    "PlaceSuggestion.Unauthenticated",
                    "Authentication is required."));
        }

        var suggestion = await _repository.GetByIdAsync(
            request.SuggestionId,
            cancellationToken);

        if (suggestion is null || suggestion.UserId != userId)
        {
            return Result.Failure(
                Error.NotFound(
                    "PlaceSuggestion.NotFound",
                    "Place suggestion was not found."));
        }

        var deleteResult = suggestion.Delete(userId);

        if (deleteResult.IsFailure)
        {
            return Result.Failure(deleteResult.Error);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}