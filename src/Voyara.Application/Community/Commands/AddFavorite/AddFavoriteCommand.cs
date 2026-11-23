
using Voyara.Application.Abstractions.Authorization;
using Voyara.Application.Abstractions.Messaging;

namespace Voyara.Application.Community.Commands.AddFavorite;

/// <summary>
/// Adds a destination to the current user's favorites.
/// </summary>
[AuthorizeRequest(Roles.Tourist)]
public sealed record AddFavoriteCommand(Guid DestinationId)
    : ICommand<Guid>;
