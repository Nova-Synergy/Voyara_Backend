using Voyara.Application.Abstractions.Authorization;
using Voyara.Application.Abstractions.Messaging;
using Voyara.Application.Community.DTOs;
using Voyara.Application.DTOs;

namespace Voyara.Application.Community.Queries.GetMyFavorites;

/// <summary>
/// Query to retrieve a paginated list of favorite destinations for the currently authenticated tourist.
/// </summary>
/// <param name="Page">The 1-based page number to retrieve. Defaults to <c>1</c>.</param>
/// <param name="PageSize">The maximum number of favorite items per page. Defaults to <c>10</c>.</param>
[AuthorizeRequest(Roles.Tourist)]
public sealed record GetMyFavoritesQuery(
    int Page = 1,
    int PageSize = 10)
    : IQuery<PagedResult<FavoriteResponse>>;