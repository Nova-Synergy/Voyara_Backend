using Voyara.SharedKernel.Results;

namespace Voyara.Domain.Entities.Community;

/// <summary>Expected business errors of the <see cref="Favorite"/> aggregate.</summary>
public static class FavoriteErrors
{
    /// <summary>The user id is missing.</summary>
    public static readonly Error UserRequired =
        Error.Validation("Favorite.UserRequired", "The user is required.");

    /// <summary>The destination id is missing.</summary>
    public static readonly Error DestinationRequired =
        Error.Validation("Favorite.DestinationRequired", "The destination is required.");

    /// <summary>The favorite was already removed.</summary>
    public static readonly Error AlreadyRemoved =
        Error.Conflict("Favorite.AlreadyRemoved", "The favorite was already removed.");

    /// <summary>The favorite is active, so it cannot be restored.</summary>
    public static readonly Error NotRemoved =
        Error.Conflict("Favorite.NotRemoved", "The favorite is not removed, so it cannot be restored.");
}