using Voyara_DomainModel.Common;

namespace Voyara.Domain.Community.PlaceSuggestions;

/// <summary>Raised when a tourist suggests a new place.</summary>
/// <param name="SuggestionId">The new suggestion.</param>
/// <param name="UserId">The tourist who suggested it.</param>
public sealed record PlaceSuggestionSubmitted(Guid SuggestionId, Guid UserId) : DomainEvent;

/// <summary>Raised when an admin approves a suggestion.</summary>
/// <param name="SuggestionId">The approved suggestion.</param>
/// <param name="UserId">The tourist, who should be notified.</param>
/// <param name="AdminId">The admin who approved it.</param>
public sealed record PlaceSuggestionApproved(Guid SuggestionId, Guid UserId, Guid AdminId) : DomainEvent;

/// <summary>Raised when an admin rejects a suggestion.</summary>
/// <param name="SuggestionId">The rejected suggestion.</param>
/// <param name="UserId">The tourist, who should be notified.</param>
/// <param name="AdminId">The admin who rejected it.</param>
/// <param name="Reason">Why it was rejected.</param>
public sealed record PlaceSuggestionRejected(Guid SuggestionId, Guid UserId, Guid AdminId, string Reason) : DomainEvent;