using Voyara.SharedKernel.Events;

namespace Voyara.Domain.Events.Community;

/// <summary>Raised when a review is submitted. Lets admins see it in the moderation queue.</summary>
public sealed record ReviewSubmittedDomainEvent(
    Guid EventId,
    DateTimeOffset OccurredOn,
    Guid ReviewId,
    Guid UserId,
    Guid DestinationId) : IDomainEvent;

/// <summary>Raised when a review is approved. Used to notify the author and update the destination's rating.</summary>
public sealed record ReviewApprovedDomainEvent(
    Guid EventId,
    DateTimeOffset OccurredOn,
    Guid ReviewId,
    Guid UserId,
    Guid DestinationId,
    Guid AdminId,
    int Rating) : IDomainEvent;

/// <summary>Raised when a review is rejected. Used to notify the author with the reason.</summary>
public sealed record ReviewRejectedDomainEvent(
    Guid EventId,
    DateTimeOffset OccurredOn,
    Guid ReviewId,
    Guid UserId,
    Guid AdminId,
    string Reason) : IDomainEvent;

/// <summary>Raised when a review is first flagged as reported. Lets moderation pick it up.</summary>
public sealed record ReviewReportedDomainEvent(
    Guid EventId,
    DateTimeOffset OccurredOn,
    Guid ReviewId,
    Guid ReporterId) : IDomainEvent;

/// <summary>Raised when a review is hidden from public view. Used to update the destination's rating.</summary>
public sealed record ReviewHiddenDomainEvent(
    Guid EventId,
    DateTimeOffset OccurredOn,
    Guid ReviewId,
    Guid UserId,
    Guid DestinationId,
    Guid AdminId,
    int Rating) : IDomainEvent;

/// <summary>Raised when a place suggestion is submitted. Lets admins see it in the moderation queue.</summary>
public sealed record PlaceSuggestionSubmittedDomainEvent(
    Guid EventId,
    DateTimeOffset OccurredOn,
    Guid SuggestionId,
    Guid UserId) : IDomainEvent;

/// <summary>
/// Raised when a place suggestion is approved. Carries the place data as plain values so the Catalog
/// module can create a destination and Notifications can inform the tourist.
/// </summary>
public sealed record PlaceSuggestionApprovedDomainEvent(
    Guid EventId,
    DateTimeOffset OccurredOn,
    Guid SuggestionId,
    Guid UserId,
    Guid AdminId,
    string Name,
    string Description,
    double Latitude,
    double Longitude) : IDomainEvent;

/// <summary>Raised when a place suggestion is rejected. Used to notify the tourist with the reason.</summary>
public sealed record PlaceSuggestionRejectedDomainEvent(
    Guid EventId,
    DateTimeOffset OccurredOn,
    Guid SuggestionId,
    Guid UserId,
    Guid AdminId,
    string Reason) : IDomainEvent;

/// <summary>Raised when an approved suggestion is linked to the destination created from it.</summary>
public sealed record PlaceSuggestionDestinationLinkedDomainEvent(
    Guid EventId,
    DateTimeOffset OccurredOn,
    Guid SuggestionId,
    Guid DestinationId) : IDomainEvent;

/// <summary>Raised when a destination is added to a user's favorites (or restored). Used for popularity.</summary>
public sealed record FavoriteAddedDomainEvent(
    Guid EventId,
    DateTimeOffset OccurredOn,
    Guid FavoriteId,
    Guid UserId,
    Guid DestinationId) : IDomainEvent;

/// <summary>Raised when a destination is removed from a user's favorites. Used for popularity.</summary>
public sealed record FavoriteRemovedDomainEvent(
    Guid EventId,
    DateTimeOffset OccurredOn,
    Guid FavoriteId,
    Guid UserId,
    Guid DestinationId) : IDomainEvent;