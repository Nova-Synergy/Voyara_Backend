
using Voyara.Domain.Enums;
using Voyara.SharedKernel.Entities;

namespace Voyara.Domain.Entities.Community;

/// <summary>
/// Aggregate root representing a tourist's opinion about a destination.
/// A review starts as Pending and moves through its lifecycle only via the methods on this class,
/// so illegal status changes are impossible. Author and destination belong to other modules
/// and are referenced by id only. <c>CreatedAt</c> is inherited from the audit base class.
/// </summary>
public sealed class Review : AggregateRoot
{
    /// <summary>Identifier of the tourist who wrote the review (reference to the Identity module).</summary>
    public Guid UserId { get; private set; }

    /// <summary>Identifier of the reviewed destination (reference to the Catalog module).</summary>
    public Guid DestinationId { get; private set; }

    /// <summary>Star rating from 1 to 5.</summary>
    public int Rating { get; private set; }

    /// <summary>The written opinion.</summary>
    public string Text { get; private set; } = string.Empty;

    /// <summary>The day the tourist visited the destination.</summary>
    public DateOnly VisitDate { get; private set; }

    /// <summary>Current position in the moderation lifecycle.</summary>
    public ReviewStatus Status { get; private set; } = ReviewStatus.Pending;

    /// <summary>When an admin last moderated the review; null if never.</summary>
    public DateTimeOffset? ModeratedAt { get; private set; }

    /// <summary>Reason given when the review was rejected; otherwise null.</summary>
    public string? RejectionReason { get; private set; }

    private readonly List<ReviewPhoto> _photos = new();

    public IReadOnlyCollection<ReviewPhoto> Photos =>
        _photos.AsReadOnly();

    private Review()
    {
    }
}
