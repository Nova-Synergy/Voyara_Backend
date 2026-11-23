
using Voyara.Application.Abstractions.Authorization;
using Voyara.Application.Abstractions.Messaging;
using Voyara.Application.Community.DTOs;
using Voyara.Application.DTOs;

namespace Voyara.Application.Community.Queries.GetMyPlaceSuggestions;

/// <summary>
/// Retrieves the current user's place suggestions using pagination.
/// </summary>
[AuthorizeRequest(Roles.Tourist)]
public sealed record GetMyPlaceSuggestionsQuery(
    int Page = 1,
    int PageSize = 10) : IQuery<PagedResult<PlaceSuggestionResponse>>;
