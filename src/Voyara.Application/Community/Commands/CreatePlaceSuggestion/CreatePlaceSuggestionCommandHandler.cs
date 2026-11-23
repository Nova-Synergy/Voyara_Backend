using MediatR;
using Voyara.Application.Abstractions;
using Voyara.Domain.Entities.Community;
using Voyara.Domain.Repositories.Community;
using Voyara.Domain.ValueObjects;
using Voyara.SharedKernel.Results;

namespace Voyara.Application.Community.Commands.CreatePlaceSuggestion;

/// <summary>
/// Handles the creation of a new place suggestion on behalf of an authenticated user.
/// </summary>
public sealed class CreatePlaceSuggestionCommandHandler
    : IRequestHandler<CreatePlaceSuggestionCommand, Result<Guid>>
{
    private readonly IPlaceSuggestionRepository _repository;
    private readonly ICurrentUser _currentUser;
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>
    /// Initializes a new instance of the <see cref="CreatePlaceSuggestionCommandHandler"/> class.
    /// </summary>
    /// <param name="repository">The repository used to persist place suggestion aggregates.</param>
    /// <param name="currentUser">The service providing information about the currently authenticated user.</param>
    /// <param name="unitOfWork">The unit of work used to commit changes to the database.</param>
    public CreatePlaceSuggestionCommandHandler(
        IPlaceSuggestionRepository repository,
        ICurrentUser currentUser,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _currentUser = currentUser;
        _unitOfWork = unitOfWork;
    }

    /// <summary>
    /// Processes the <see cref="CreatePlaceSuggestionCommand"/> to validate coordinates,
    /// instantiate a <see cref="PlaceSuggestion"/> aggregate, and persist it.
    /// </summary>
    /// <param name="request">The command containing the proposed place details and coordinates.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// A <see cref="Result{TValue}"/> containing the identifier of the newly created <see cref="PlaceSuggestion"/> on success;
    /// otherwise, a failure result with an unauthorized or domain validation error.
    /// </returns>
    public async Task<Result<Guid>> Handle(
        CreatePlaceSuggestionCommand request,
        CancellationToken cancellationToken)
    {
        if (_currentUser.UserId is not Guid userId)
        {
            return Result.Failure<Guid>(
                Error.Unauthorized(
                    "PlaceSuggestion.Unauthenticated",
                    "Authentication is required."));
        }

        var locationResult = GeoPoint.Create(
            request.Latitude,
            request.Longitude);

        if (locationResult.IsFailure)
        {
            return Result.Failure<Guid>(locationResult.Error);
        }

        var suggestionResult = PlaceSuggestion.Create(
            userId,
            request.Name,
            request.Description,
            locationResult.Value,
            request.Reason);

        if (suggestionResult.IsFailure)
        {
            return Result.Failure<Guid>(suggestionResult.Error);
        }

        var suggestion = suggestionResult.Value;

        _repository.Add(suggestion);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(suggestion.Id);
    }
}