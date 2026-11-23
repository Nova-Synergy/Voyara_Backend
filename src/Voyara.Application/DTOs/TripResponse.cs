namespace Voyara.Application.DTOs;

/// <summary>
/// Read model representing an overarching tourist trip.
/// </summary>
/// <param name="Id">The unique identifier of the trip.</param>
/// <param name="UserId">The identifier of the user who owns the trip.</param>
/// <param name="Title">Descriptive title of the trip.</param>
/// <param name="StartDate">Planned departure date.</param>
/// <param name="EndDate">Planned return date.</param>
/// <param name="Status">Current trip lifecycle status (e.g. Draft, Upcoming, Ongoing, Completed, Cancelled).</param>
/// <param name="Visibility">Privacy level (e.g. Private, Public, Shared).</param>
/// <param name="JourneyId">Associated journey itinerary identifier, if attached.</param>
/// <param name="Shares">Active and historical share links generated for this trip.</param>
public sealed record TripResponse(
    Guid Id,
    Guid UserId,
    string Title,
    DateOnly StartDate,
    DateOnly EndDate,
    string Status,
    string Visibility,
    Guid? JourneyId,
    IReadOnlyList<TripShareResponse> Shares);

/// <summary>
/// Read model representing a trip share token.
/// </summary>
/// <param name="Id">The unique identifier of the share record.</param>
/// <param name="Token">The secure share token string.</param>
/// <param name="ExpiresAt">Optional expiration timestamp in UTC.</param>
/// <param name="RevokedAt">Optional revocation timestamp in UTC.</param>
/// <param name="IsActive">Whether the share token is currently active.</param>
public sealed record TripShareResponse(
    Guid Id,
    string Token,
    DateTime? ExpiresAt,
    DateTime? RevokedAt,
    bool IsActive);
