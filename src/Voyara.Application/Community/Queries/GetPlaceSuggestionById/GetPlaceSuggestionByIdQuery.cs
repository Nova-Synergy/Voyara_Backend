
using Voyara.Application.Abstractions.Messaging;
using Voyara.Application.Community.DTOs;
using Voyara.Application.Abstractions.Authorization;

namespace Voyara.Application.Community.Queries.GetPlaceSuggestionById;

/// <summary>
/// Retrieves a place suggestion by its identifier.
/// </summary>
/// <param name="SuggestionId">The identifier of the requested suggestion.</param>
[AuthorizeRequest(Roles.Tourist)]
public sealed record GetPlaceSuggestionByIdQuery(Guid SuggestionId)
    : IQuery<PlaceSuggestionResponse>;
