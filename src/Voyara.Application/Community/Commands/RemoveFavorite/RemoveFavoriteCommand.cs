
using Voyara.Application.Abstractions.Authorization;
using Voyara.Application.Abstractions.Messaging;

namespace Voyara.Application.Community.Commands.RemoveFavorite;

/// <summary>
/// Removes a destination from the current user's favorites.
/// </summary>
[AuthorizeRequest(Roles.Tourist)]
public sealed record RemoveFavoriteCommand(Guid DestinationId)
    : ICommand;
