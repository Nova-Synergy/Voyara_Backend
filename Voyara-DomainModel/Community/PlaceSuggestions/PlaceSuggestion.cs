
using Voyara_DomainModel.Common;
using Voyara_DomainModel.Community.PlaceSuggestions;

namespace Voyara.Domain.Community.PlaceSuggestions;

/// <summary>
/// A tourist's proposal for a new tourism place (aggregate root).
/// It starts as <see cref="SuggestionStatus.Pending"/>; an admin then approves or rejects it.
/// Once approved, it can be linked to the Destination created from it in the Catalog module.
/// </summary>
public sealed class PlaceSuggestion : AggregateRoot<Guid>
{
    private readonly List<SuggestionPhoto> _photos = new();

    private PlaceSuggestion() { } // Required by EF Core.

    /// <summary>Id of the tourist who made the suggestion (a reference to the Identity module).</summary>
    public Guid UserId { get; private set; }

    /// <summary>Proposed name of the place.</summary>
    public string Name { get; private set; } = default!;

    /// <summary>Description of the place.</summary>
    public string Description { get; private set; } = default!;

    /// <summary>Where the place is on the map.</summary>
    public GeoPoint Location { get; private set; } = default!;

    /// <summary>Why the tourist thinks the place should be on the platform.</summary>
    public string Reason { get; private set; } = default!;

    /// <summary>The current decision state.</summary>
    public SuggestionStatus Status { get; private set; }

    /// <summary>The admin's explanation when the suggestion was rejected, otherwise <c>null</c>.</summary>
    public string? DecisionReason { get; private set; }

    /// <summary>
    /// Id of the Destination (Catalog module) created from this suggestion,
    /// or <c>null</c> if it has not become one (the diagram's 0..1 relationship).
    /// </summary>
    public Guid? DestinationId { get; private set; }

    /// <summary>Photos attached to the suggestion (read-only).</summary>
    public IReadOnlyCollection<SuggestionPhoto> Photos => _photos;

    /// <summary>Creates a new suggestion in the <see cref="SuggestionStatus.Pending"/> state.</summary>
    /// <param name="userId">The suggesting tourist's id.</param>
    /// <param name="name">Name of the place (required).</param>
    /// <param name="description">Description of the place (required).</param>
    /// <param name="location">Geographic position of the place.</param>
    /// <param name="reason">Why it should be added (required).</param>
    /// <param name="photoUrls">Optional photo URLs.</param>
    /// <returns>A valid, pending suggestion.</returns>
    /// <exception cref="DomainException">Thrown when any rule is broken.</exception>
    public static PlaceSuggestion Create(Guid userId, string name, string description,
        GeoPoint location, string reason, IEnumerable<string>? photoUrls = null)
    {
        if (userId == Guid.Empty) throw new DomainException("UserId is required.");
        if (string.IsNullOrWhiteSpace(name)) throw new DomainException("Name is required.");
        if (string.IsNullOrWhiteSpace(description)) throw new DomainException("Description is required.");
        if (string.IsNullOrWhiteSpace(reason)) throw new DomainException("Reason is required.");
        ArgumentNullException.ThrowIfNull(location);

        var suggestion = new PlaceSuggestion
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Name = name.Trim(),
            Description = description.Trim(),
            Location = location,
            Reason = reason.Trim(),
            Status = SuggestionStatus.Pending
        };

        if (photoUrls is not null)
            suggestion._photos.AddRange(photoUrls.Select(SuggestionPhoto.Create));

        suggestion.Raise(new PlaceSuggestionSubmitted(suggestion.Id, userId));
        return suggestion;
    }

    /// <summary>Approves a pending suggestion.</summary>
    /// <param name="adminId">The admin making the decision.</param>
    /// <exception cref="DomainException">Thrown when the suggestion is not Pending.</exception>
    public void Approve(Guid adminId)
    {
        EnsurePending("approve");

        Status = SuggestionStatus.Approved;
        Raise(new PlaceSuggestionApproved(Id, UserId, adminId));
    }

    /// <summary>Rejects a pending suggestion and records why.</summary>
    /// <param name="adminId">The admin making the decision.</param>
    /// <param name="reason">The explanation shown to the tourist (required).</param>
    /// <exception cref="DomainException">Thrown when the reason is empty or the suggestion is not Pending.</exception>
    public void Reject(Guid adminId, string reason)
    {
        if (string.IsNullOrWhiteSpace(reason))
            throw new DomainException("A rejection reason is required.");
        EnsurePending("reject");

        Status = SuggestionStatus.Rejected;
        DecisionReason = reason.Trim();
        Raise(new PlaceSuggestionRejected(Id, UserId, adminId, DecisionReason));
    }

    /// <summary>
    /// Links this suggestion to the Destination that was created from it.
    /// Called by the application layer after the Catalog module creates the destination.
    /// </summary>
    /// <param name="destinationId">The id of the newly created destination.</param>
    /// <exception cref="DomainException">Thrown when the suggestion is not Approved or is already linked.</exception>
    public void LinkDestination(Guid destinationId)
    {
        if (Status != SuggestionStatus.Approved)
            throw new DomainException("Only approved suggestions can become destinations.");
        if (DestinationId is not null)
            throw new DomainException("Suggestion is already linked to a destination.");

        DestinationId = destinationId;
    }

    /// <summary>Throws unless the suggestion is still waiting for a decision.</summary>
    private void EnsurePending(string operation)
    {
        if (Status != SuggestionStatus.Pending)
            throw new DomainException($"Cannot {operation} a suggestion that is {Status}.");
    }
}