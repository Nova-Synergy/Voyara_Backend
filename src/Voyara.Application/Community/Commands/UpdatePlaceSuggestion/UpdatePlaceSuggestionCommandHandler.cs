
using MediatR;
using Voyara.Application.Abstractions;
using Voyara.Domain.Repositories.Community;
using Voyara.Domain.ValueObjects;
using Voyara.SharedKernel.Results;

namespace Voyara.Application.Community.Commands.UpdatePlaceSuggestion;

/// <summary>
/// Handles updating an existing place suggestion owned by the currently authenticated user.
/// </summary>
public sealed class UpdatePlaceSuggestionCommandHandler
    : IRequestHandler<UpdatePlaceSuggestionCommand, Result>
{
    private readonly IPlaceSuggestionRepository _repository;
    private readonly ICurrentUser _currentUser;
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdatePlaceSuggestionCommandHandler"/> class.
    /// </summary>
    /// <param name="repository">The repository used to query and persist place suggestion aggregates.</param>
    /// <param name="currentUser">The service providing information about the currently authenticated user.</param>
    /// <param name="unitOfWork">The unit of work used to commit changes to the database.</param>
    public UpdatePlaceSuggestionCommandHandler(
        IPlaceSuggestionRepository repository,
        ICurrentUser currentUser,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _currentUser = currentUser;
        _unitOfWork = unitOfWork;
    }

    /// <inheritdoc />
    public async Task<Result> Handle(
        UpdatePlaceSuggestionCommand request,
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

        var locationResult = GeoPoint.Create(
            request.Latitude,
            request.Longitude);

        if (locationResult.IsFailure)
        {
            return Result.Failure(locationResult.Error);
        }

        var updateResult = suggestion.Update(
            userId,
            request.Name,
            request.Description,
            locationResult.Value,
            request.Reason);

        if (updateResult.IsFailure)
        {
            return Result.Failure(updateResult.Error);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
