
using MediatR;
using Voyara.Application.Abstractions;
using Voyara.Application.Community.DTOs;
using Voyara.Application.DTOs;
using Voyara.Domain.Repositories.Community;
using Voyara.SharedKernel.Results;

namespace Voyara.Application.Community.Queries.GetMyPlaceSuggestions;

/// <summary>
/// Handles retrieving the current user's paginated place suggestions.
/// </summary>
public sealed class GetMyPlaceSuggestionsQueryHandler
    : IRequestHandler<
        GetMyPlaceSuggestionsQuery,
        Result<PagedResult<PlaceSuggestionResponse>>>
{
    private readonly IPlaceSuggestionRepository _repository;
    private readonly ICurrentUser _currentUser;

    /// <summary>
    /// Initializes a new instance of the handler.
    /// </summary>
    public GetMyPlaceSuggestionsQueryHandler(
        IPlaceSuggestionRepository repository,
        ICurrentUser currentUser)
    {
        _repository = repository;
        _currentUser = currentUser;
    }

    /// <inheritdoc />
    public async Task<Result<PagedResult<PlaceSuggestionResponse>>> Handle(
        GetMyPlaceSuggestionsQuery request,
        CancellationToken cancellationToken)
    {
        if (_currentUser.UserId is not Guid userId)
        {
            return Result.Failure<PagedResult<PlaceSuggestionResponse>>(
                Error.Unauthorized(
                    "PlaceSuggestion.Unauthenticated",
                    "Authentication is required."));
        }

        var page = Math.Max(1, request.Page);
        var pageSize = Math.Clamp(request.PageSize, 1, 100);

        var skip = (int)Math.Min(
            (long)(page - 1) * pageSize,
            int.MaxValue);

        var suggestions = await _repository.GetByUserAsync(
            userId,
            skip,
            pageSize,
            cancellationToken);

        var totalCount = await _repository.CountByUserIdAsync(
            userId,
            cancellationToken);

        IReadOnlyList<PlaceSuggestionResponse> items = suggestions
            .Select(PlaceSuggestionResponse.From)
            .ToList();

        var response = new PagedResult<PlaceSuggestionResponse>(
            items,
            totalCount,
            page,
            pageSize);

        return Result.Success(response);
    }
}
