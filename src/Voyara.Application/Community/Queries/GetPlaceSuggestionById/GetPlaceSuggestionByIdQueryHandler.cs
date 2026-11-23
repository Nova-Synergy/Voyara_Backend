
using MediatR;
using Voyara.Application.Abstractions;
using Voyara.Application.Community.DTOs;
using Voyara.Domain.Repositories.Community;
using Voyara.SharedKernel.Results;

namespace Voyara.Application.Community.Queries.GetPlaceSuggestionById;

/// <summary>
/// Handles retrieving a single place suggestion owned by the currently authenticated tourist.
/// </summary>
public sealed class GetPlaceSuggestionByIdQueryHandler
    : IRequestHandler<
        GetPlaceSuggestionByIdQuery,
        Result<PlaceSuggestionResponse>>
{
    private readonly IPlaceSuggestionRepository _repository;
    private readonly ICurrentUser _currentUser;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetPlaceSuggestionByIdQueryHandler"/> class.
    /// </summary>
    /// <param name="repository">The repository used to query place suggestion aggregates.</param>
    /// <param name="currentUser">The service providing information about the currently authenticated user.</param>
    public GetPlaceSuggestionByIdQueryHandler(
        IPlaceSuggestionRepository repository,
        ICurrentUser currentUser)
    {
        _repository = repository;
        _currentUser = currentUser;
    }

    /// <inheritdoc />
    public async Task<Result<PlaceSuggestionResponse>> Handle(
        GetPlaceSuggestionByIdQuery request,
        CancellationToken cancellationToken)
    {
        if (_currentUser.UserId is not Guid userId)
        {
            return Result.Failure<PlaceSuggestionResponse>(
                Error.Unauthorized(
                    "PlaceSuggestion.Unauthenticated",
                    "Authentication is required."));
        }

        var suggestion = await _repository.GetByIdAsync(
            request.SuggestionId,
            cancellationToken);

        if (suggestion is null || suggestion.UserId != userId)
        {
            return Result.Failure<PlaceSuggestionResponse>(
                Error.NotFound(
                    "PlaceSuggestion.NotFound",
                    "Place suggestion was not found."));
        }

        return Result.Success(PlaceSuggestionResponse.From(suggestion));
    }
}
