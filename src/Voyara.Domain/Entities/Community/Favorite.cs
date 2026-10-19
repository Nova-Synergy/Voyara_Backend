using System.Security.Cryptography;
using Voyara.Domain.Events.Community;
using Voyara.SharedKernel.Entities;
using Voyara.SharedKernel.Results;

namespace Voyara.Domain.Entities.Community;

/// <summary>
/// A destination saved by a tourist (a small aggregate root). The user and the destination belong to
/// other modules and are referenced by id only.
/// </summary>
/// <remarks>
/// Identity rule: the favorite's <c>Id</c> is derived from the pair (user, destination) by
/// <see cref="ComputeId"/>. The same pair therefore always has the same identity, so a user cannot hold
/// the same destination twice (the database primary key rejects a duplicate), and the application can
/// look a favorite up directly by the pair. Removing a favorite is a soft delete (the platform's
/// <c>IsDeleted</c> flag), and <see cref="Restore"/> brings it back instead of creating a second row.
/// </remarks>
public sealed class Favorite : AggregateRoot
{
    /// <summary>Identifier of the tourist who saved the destination (reference to the Identity module).</summary>
    public Guid UserId { get; private set; }

    /// <summary>Identifier of the saved destination (reference to the Catalog module).</summary>
    public Guid DestinationId { get; private set; }

    private Favorite()
    {
    }

    private Favorite(Guid userId, Guid destinationId)
    {
        Id = ComputeId(userId, destinationId);
        UserId = userId;
        DestinationId = destinationId;
    }

    /// <summary>
    /// Computes the identity of the favorite for a user and a destination. The result is stable: the same
    /// pair always gives the same id, and swapping the two ids gives a different one.
    /// </summary>
    public static Guid ComputeId(Guid userId, Guid destinationId)
    {
        Span<byte> input = stackalloc byte[32];
        userId.TryWriteBytes(input[..16]);
        destinationId.TryWriteBytes(input[16..]);

        Span<byte> hash = stackalloc byte[32];
        SHA256.HashData(input, hash);

        return new Guid(hash[..16]);
    }

    /// <summary>
    /// Creates a favorite after validating its ids, and raises <see cref="FavoriteAddedDomainEvent"/>
    /// (used, for example, to update destination popularity).
    /// </summary>
    /// <returns>The new favorite, or the rule that was broken.</returns>
    public static Result<Favorite> Create(Guid userId, Guid destinationId)
    {
        if (userId == Guid.Empty)
        {
            return Result.Failure<Favorite>(FavoriteErrors.UserRequired);
        }

        if (destinationId == Guid.Empty)
        {
            return Result.Failure<Favorite>(FavoriteErrors.DestinationRequired);
        }

        var favorite = new Favorite(userId, destinationId);

        favorite.RaiseDomainEvent(new FavoriteAddedDomainEvent(
            Guid.NewGuid(), DateTimeOffset.UtcNow, favorite.Id, userId, destinationId));

        return favorite;
    }

    /// <summary>Removes the favorite (soft delete) and raises <see cref="FavoriteRemovedDomainEvent"/>.</summary>
    public Result Remove()
    {
        if (IsDeleted)
        {
            return Result.Failure(FavoriteErrors.AlreadyRemoved);
        }

        IsDeleted = true;

        RaiseDomainEvent(new FavoriteRemovedDomainEvent(
            Guid.NewGuid(), DateTimeOffset.UtcNow, Id, UserId, DestinationId));

        return Result.Success();
    }

    /// <summary>
    /// Brings a removed favorite back, for when the user saves the same destination again. Raises
    /// <see cref="FavoriteAddedDomainEvent"/>.
    /// </summary>
    public Result Restore()
    {
        if (!IsDeleted)
        {
            return Result.Failure(FavoriteErrors.NotRemoved);
        }

        IsDeleted = false;

        RaiseDomainEvent(new FavoriteAddedDomainEvent(
            Guid.NewGuid(), DateTimeOffset.UtcNow, Id, UserId, DestinationId));

        return Result.Success();
    }
}