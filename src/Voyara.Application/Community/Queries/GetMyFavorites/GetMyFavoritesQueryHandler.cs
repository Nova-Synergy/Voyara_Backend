using MediatR;
using Voyara.Application.Abstractions;
using Voyara.Application.Community.DTOs;
using Voyara.Application.DTOs;
using Voyara.Domain.Repositories.Community;
using Voyara.SharedKernel.Results;

namespace Voyara.Application.Community.Queries.GetMyFavorites;

/// <summary>
/// Handles retrieval of the currently authenticated user's paginated favorite destinations.
/// </summary>
public sealed class GetMyFavoritesQueryHandler
    : IRequestHandler<
        GetMyFavoritesQuery,
        Result<PagedResult<FavoriteResponse>>>
{
    private readonly IFavoriteRepository _repository;
    private readonly ICurrentUser _currentUser;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetMyFavoritesQueryHandler"/> class.
    /// </summary>
    /// <param name="repository">The repository used to query favorite entities and counts.</param>
    /// <param name="currentUser">The service providing information about the currently authenticated user.</param>
    public GetMyFavoritesQueryHandler(
        IFavoriteRepository repository,
        ICurrentUser currentUser)
    {
        _repository = repository;
        _currentUser = currentUser;
    }

    /// <summary>
    /// Processes the <see cref="GetMyFavoritesQuery"/> to return a paginated list of the user's favorites.
    /// </summary>
    /// <param name="request">The query specifying pagination parameters.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// A <see cref="Result{TValue}"/> containing a <see cref="PagedResult{T}"/> of <see cref="FavoriteResponse"/>
    /// items on success; otherwise, an unauthorized error result.
    /// </returns>
    public async Task<Result<PagedResult<FavoriteResponse>>> Handle(
        GetMyFavoritesQuery request,
        CancellationToken cancellationToken)
    {
        if (_currentUser.UserId is not Guid userId)
        {
            return Result.Failure<PagedResult<FavoriteResponse>>(
                Error.Unauthorized(
                    "Favorite.Unauthenticated",
                    "Authentication is required."));
        }

        var page = Math.Max(1, request.Page);
        var pageSize = Math.Clamp(request.PageSize, 1, 100);

        var skip = (int)Math.Min(
            (long)(page - 1) * pageSize,
            int.MaxValue);

        var favorites = await _repository.GetByUserIdAsync(
            userId,
            skip,
            pageSize,
            cancellationToken);

        var totalCount = await _repository.CountByUserIdAsync(
            userId,
            cancellationToken);

        IReadOnlyList<FavoriteResponse> items = favorites
            .Select(FavoriteResponse.From)
            .ToList();

        var response = new PagedResult<FavoriteResponse>(
            items,
            totalCount,
            page,
            pageSize);

        return Result.Success(response);
    }
}