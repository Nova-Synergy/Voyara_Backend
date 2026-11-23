
using Voyara.Domain.Entities.Community;

namespace Voyara.Application.Community.DTOs;

/// <summary>
/// Represents a favorite destination returned to the client.
/// </summary>
public sealed record FavoriteResponse(
    Guid Id,
    Guid DestinationId)
{
    /// <summary>
    /// Creates a response from a Favorite aggregate.
    /// </summary>
    public static FavoriteResponse From(Favorite favorite)
    {
        return new FavoriteResponse(
            favorite.Id,
            favorite.DestinationId);
    }
}
