using Voyara.SharedKernel.Events;

namespace Voyara.Domain.Events.Community;

/// <summary>Raised when a review is submitted.</summary>
public sealed record ReviewSubmittedDomainEvent(
    Guid EventId,
    DateTimeOffset OccurredOn,
    Guid ReviewId,
    Guid UserId,
    Guid DestinationId) : IDomainEvent;

/// <summary>Raised when a review is approved.</summary>
public sealed record ReviewApprovedDomainEvent(
    Guid EventId,
    DateTimeOffset OccurredOn,
    Guid ReviewId,
    Guid UserId,
    Guid DestinationId,
    Guid AdminId) : IDomainEvent;

/// <summary>Raised when a review is rejected.</summary>
public sealed record ReviewRejectedDomainEvent(
    Guid EventId,
    DateTimeOffset OccurredOn,
    Guid ReviewId,
    Guid UserId,
    Guid AdminId,
    string Reason) : IDomainEvent;

/// <summary>Raised when a review is reported for moderation.</summary>
public sealed record ReviewReportedDomainEvent(
    Guid EventId,
    DateTimeOffset OccurredOn,
    Guid ReviewId,
    Guid ReporterId) : IDomainEvent;

/// <summary>Raised when a review is hidden from public view.</summary>
public sealed record ReviewHiddenDomainEvent(
    Guid EventId,
    DateTimeOffset OccurredOn,
    Guid ReviewId,
    Guid UserId,
    Guid AdminId) : IDomainEvent;

/// <summary>Raised when a place suggestion is submitted.</summary>
public sealed record PlaceSuggestionSubmittedDomainEvent(
    Guid EventId,
    DateTimeOffset OccurredOn,
    Guid SuggestionId,
    Guid UserId) : IDomainEvent;

/// <summary>Raised when a place suggestion is approved.</summary>
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

/// <summary>Raised when a place suggestion is rejected.</summary>
public sealed record PlaceSuggestionRejectedDomainEvent(
    Guid EventId,
    DateTimeOffset OccurredOn,
    Guid SuggestionId,
    Guid UserId,
    Guid AdminId,
    string Reason) : IDomainEvent;

/// <summary>Raised when an approved suggestion is linked to a destination.</summary>
public sealed record PlaceSuggestionDestinationLinkedDomainEvent(
    Guid EventId,
    DateTimeOffset OccurredOn,
    Guid SuggestionId,
    Guid DestinationId) : IDomainEvent;

/// <summary>Raised when a destination is added to a user's favorites.</summary>
public sealed record FavoriteAddedDomainEvent(
    Guid EventId,
    DateTimeOffset OccurredOn,
    Guid FavoriteId,
    Guid UserId,
    Guid DestinationId) : IDomainEvent;