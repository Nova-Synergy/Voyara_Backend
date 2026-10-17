
using Voyara.SharedKernel.Entities;

namespace Voyara.Domain.Entities.Community;

/// <summary>
/// Represents a destination saved by a tourist.
/// References the user and destination by their identifiers.
/// </summary>
public sealed class Favorite : AuditableEntity
{
    /// <summary>
    /// Identifier of the tourist who saved the destination.
    /// </summary>
    public Guid UserId { get; private set; }

    /// <summary>
    /// Identifier of the saved destination.
    /// </summary>
    public Guid DestinationId { get; private set; }

    private Favorite()
    {
    }
}
