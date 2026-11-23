
using Voyara.Application.Abstractions.Authorization;
using Voyara.Application.Abstractions.Messaging;

namespace Voyara.Application.Community.Commands.DeletePlaceSuggestion;

/// <summary>
/// Soft-deletes a place suggestion belonging to the current user.
/// </summary>
/// <param name="SuggestionId">The identifier of the suggestion to delete.</param>
[AuthorizeRequest(Roles.Tourist)]
public sealed record DeletePlaceSuggestionCommand(Guid SuggestionId)
    : ICommand;
