using MediatR;
using Voyara.Application.Abstractions;
using Voyara.Domain.Repositories.Community;
using Voyara.SharedKernel.Results;

namespace Voyara.Application.Community.Commands.RemoveFavorite;

/// <summary>
/// Handles removing a destination from the authenticated user's favorites.
/// </summary>
public sealed class RemoveFavoriteCommandHandler
    : IRequestHandler<RemoveFavoriteCommand, Result>
{
    private readonly IFavoriteRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUser _currentUser;

    /// <summary>
    /// Initializes a new instance of the <see cref="RemoveFavoriteCommandHandler"/> class.
    /// </summary>
    /// <param name="repository">The repository used to query and manage favorite entities.</param>
    /// <param name="unitOfWork">The unit of work used to commit changes to the database.</param>
    /// <param name="currentUser">The service providing information about the currently authenticated user.</param>
    public RemoveFavoriteCommandHandler(
        IFavoriteRepository repository,
        IUnitOfWork unitOfWork,
        ICurrentUser currentUser)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }

    /// <summary>
    /// Processes the <see cref="RemoveFavoriteCommand"/> to remove a user's favorite destination.
    /// </summary>
    /// <param name="request">The command containing the target destination identifier.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// A <see cref="Result"/> indicating success if the favorite was removed;
    /// otherwise, a failure result with an unauthorized, not-found, or domain error.
    /// </returns>
    public async Task<Result> Handle(
        RemoveFavoriteCommand request,
        CancellationToken cancellationToken)
    {
        if (_currentUser.UserId is not Guid userId)
        {
            return Result.Failure(
                Error.Unauthorized(
                    "Favorite.Unauthenticated",
                    "Authentication is required."));
        }

        var favorite = await _repository.GetByUserAndDestinationAsync(
            userId,
            request.DestinationId,
            cancellationToken);

        if (favorite is null)
        {
            return Result.Failure(
                Error.NotFound(
                    "Favorite.NotFound",
                    "Favorite was not found."));
        }

        var removeResult = favorite.Remove();

        if (removeResult.IsFailure)
        {
            return removeResult;
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}