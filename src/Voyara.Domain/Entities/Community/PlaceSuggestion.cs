using Voyara.Domain.Enums;
using Voyara.Domain.Events.Community;
using Voyara.Domain.ValueObjects;
using Voyara.SharedKernel.Entities;
using Voyara.SharedKernel.Results;

namespace Voyara.Domain.Entities.Community;

/// <summary>
/// Aggregate root representing a tourist's proposal for a new tourism place.
/// </summary>
public sealed class PlaceSuggestion : AggregateRoot
{
    private readonly List<SuggestionPhoto> _photos = [];

    /// <summary>Identifier of the submitting tourist.</summary>
    public Guid UserId { get; private set; }

    /// <summary>Proposed place name.</summary>
    public string Name { get; private set; } = string.Empty;

    /// <summary>Description of the proposed place.</summary>
    public string Description { get; private set; } = string.Empty;

    /// <summary>Geographical location of the proposed place.</summary>
    public GeoPoint Location { get; private set; } = null!;

    /// <summary>Reason for suggesting the place.</summary>
    public string Reason { get; private set; } = string.Empty;

    /// <summary>Current moderation status.</summary>
    public SuggestionStatus Status { get; private set; } = SuggestionStatus.Pending;

    /// <summary>Explanation for the moderation decision, if applicable.</summary>
    public string? DecisionReason { get; private set; }

    /// <summary>Destination created from this suggestion, if linked.</summary>
    public Guid? DestinationId { get; private set; }

    /// <summary>Photos managed by this suggestion aggregate.</summary>
    public IReadOnlyCollection<SuggestionPhoto> Photos => _photos.AsReadOnly();

    private PlaceSuggestion() { }

    private PlaceSuggestion(
        Guid userId,
        string name,
        string description,
        GeoPoint location,
        string reason)
    {
        UserId = userId;
        Name = name;
        Description = description;
        Location = location;
        Reason = reason;
        Status = SuggestionStatus.Pending;
    }

    /// <summary>Creates a suggestion after validating the required information.</summary>
    public static Result<PlaceSuggestion> Create(
        Guid userId,
        string name,
        string description,
        GeoPoint location,
        string reason)
    {
        if (userId == Guid.Empty)
            return Result.Failure<PlaceSuggestion>(PlaceSuggestionErrors.UserRequired);

        if (string.IsNullOrWhiteSpace(name))
            return Result.Failure<PlaceSuggestion>(PlaceSuggestionErrors.NameRequired);

        if (string.IsNullOrWhiteSpace(description))
            return Result.Failure<PlaceSuggestion>(PlaceSuggestionErrors.DescriptionRequired);

        if (location is null)
            return Result.Failure<PlaceSuggestion>(PlaceSuggestionErrors.LocationRequired);

        if (string.IsNullOrWhiteSpace(reason))
            return Result.Failure<PlaceSuggestion>(PlaceSuggestionErrors.ReasonRequired);

        var suggestion = new PlaceSuggestion(
            userId, name.Trim(), description.Trim(), location, reason.Trim());

        suggestion.RaiseDomainEvent(new PlaceSuggestionSubmittedDomainEvent(
            Guid.NewGuid(), DateTimeOffset.UtcNow, suggestion.Id, suggestion.UserId));

        return suggestion;
    }

    /// <summary>Adds a photo to this suggestion.</summary>
    public Result AddPhoto(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
            return Result.Failure(PlaceSuggestionErrors.InvalidPhotoUrl);

        _photos.Add(SuggestionPhoto.Create(Id, url));
        return Result.Success();
    }

    /// <summary>Removes a photo owned by this suggestion.</summary>
    public Result RemovePhoto(Guid photoId)
    {
        var photo = _photos.FirstOrDefault(p => p.Id == photoId);

        if (photo is null)
            return Result.Failure(PlaceSuggestionErrors.PhotoNotFound);

        _photos.Remove(photo);
        return Result.Success();
    }

    /// <summary>Approves a suggestion that is awaiting a decision.</summary>
    public Result Approve(Guid adminId)
    {
        if (adminId == Guid.Empty)
            return Result.Failure(PlaceSuggestionErrors.AdminRequired);

        if (Status != SuggestionStatus.Pending)
            return Result.Failure(
                PlaceSuggestionErrors.InvalidTransition("approve", Status));

        Status = SuggestionStatus.Approved;
        DecisionReason = null;

        RaiseDomainEvent(new PlaceSuggestionApprovedDomainEvent(
            Guid.NewGuid(), DateTimeOffset.UtcNow, Id, UserId, adminId,
            Name, Description, Location.Latitude, Location.Longitude));

        return Result.Success();
    }

    /// <summary>Rejects a pending suggestion with a reason.</summary>
    public Result Reject(Guid adminId, string reason)
    {
        if (adminId == Guid.Empty)
            return Result.Failure(PlaceSuggestionErrors.AdminRequired);

        if (string.IsNullOrWhiteSpace(reason))
            return Result.Failure(PlaceSuggestionErrors.DecisionReasonRequired);

        if (Status != SuggestionStatus.Pending)
            return Result.Failure(
                PlaceSuggestionErrors.InvalidTransition("reject", Status));

        Status = SuggestionStatus.Rejected;
        DecisionReason = reason.Trim();

        RaiseDomainEvent(new PlaceSuggestionRejectedDomainEvent(
            Guid.NewGuid(), DateTimeOffset.UtcNow, Id, UserId, adminId, DecisionReason));

        return Result.Success();
    }

    /// <summary>Links an approved suggestion to its resulting destination.</summary>
    public Result LinkDestination(Guid destinationId)
    {
        if (destinationId == Guid.Empty)
            return Result.Failure(PlaceSuggestionErrors.DestinationRequired);

        if (Status != SuggestionStatus.Approved)
            return Result.Failure(PlaceSuggestionErrors.NotApproved);

        if (DestinationId.HasValue)
            return Result.Failure(PlaceSuggestionErrors.AlreadyLinked);

        DestinationId = destinationId;

        RaiseDomainEvent(new PlaceSuggestionDestinationLinkedDomainEvent(
            Guid.NewGuid(), DateTimeOffset.UtcNow, Id, destinationId));

        return Result.Success();
    }
}