
using MediatR;
using Voyara.Application.Abstractions;
using Voyara.Domain.Entities.Community;
using Voyara.Domain.Repositories.Community;
using Voyara.SharedKernel.Results;

namespace Voyara.Application.Community.Commands.AddFavorite;

/// <summary>
/// Handles adding a destination to the authenticated user's favorites,
/// restoring a previously soft-deleted favorite if one exists.
/// </summary>
public sealed class AddFavoriteCommandHandler
    : IRequestHandler<AddFavoriteCommand, Result<Guid>>
{
    private readonly IFavoriteRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUser _currentUser;

    /// <summary>
    /// Initializes a new instance of the <see cref="AddFavoriteCommandHandler"/> class.
    /// </summary>
    /// <param name="repository">The repository used to query and persist favorite entities.</param>
    /// <param name="unitOfWork">The unit of work used to commit changes to the database.</param>
    /// <param name="currentUser">The service providing information about the currently authenticated user.</param>
    public AddFavoriteCommandHandler(
        IFavoriteRepository repository,
        IUnitOfWork unitOfWork,
        ICurrentUser currentUser)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }

    /// <summary>
    /// Processes the <see cref="AddFavoriteCommand"/> to create or restore a user's favorite destination.
    /// </summary>
    /// <param name="request">The command containing the target destination identifier.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// A <see cref="Result{TValue}"/> containing the identifier of the created or restored <see cref="Favorite"/> on success;
    /// otherwise, a failure result with an unauthorized, conflict, or domain validation error.
    /// </returns>
    public async Task<Result<Guid>> Handle(
        AddFavoriteCommand request,
        CancellationToken cancellationToken)
    {
        if (_currentUser.UserId is not Guid userId)
        {
            return Result.Failure<Guid>(
                Error.Unauthorized(
                    "Favorite.Unauthenticated",
                    "Authentication is required."));
        }

        var existingFavorite =
            await _repository.GetByUserAndDestinationAsync(
                userId,
                request.DestinationId,
                cancellationToken);

        if (existingFavorite is not null)
        {
            if (!existingFavorite.IsDeleted)
            {
                return Result.Failure<Guid>(
                    Error.Conflict(
                        "Favorite.AlreadyExists",
                        "This destination is already in your favorites."));
            }

            var restoreResult = existingFavorite.Restore();

            if (restoreResult.IsFailure)
            {
                return Result.Failure<Guid>(restoreResult.Error);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success(existingFavorite.Id);
        }

        var creationResult = Favorite.Create(
            userId,
            request.DestinationId);

        if (creationResult.IsFailure)
        {
            return Result.Failure<Guid>(creationResult.Error);
        }

        _repository.Add(creationResult.Value);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(creationResult.Value.Id);
    }
}
