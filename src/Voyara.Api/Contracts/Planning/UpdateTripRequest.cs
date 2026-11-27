using Voyara.Domain.Enums;

namespace Voyara.Api.Contracts.Planning;

/// <summary>
/// Request contract for updating a trip's details, schedule, or visibility.
/// </summary>
/// <param name="Title">Revised descriptive title of the trip.</param>
/// <param name="StartDate">Revised planned arrival/start date.</param>
/// <param name="EndDate">Revised planned departure/end date.</param>
/// <param name="Visibility">Revised privacy and sharing scope.</param>
/// <param name="Status">Optional new lifecycle status progression.</param>
public sealed record UpdateTripRequest(
    string Title,
    DateOnly StartDate,
    DateOnly EndDate,
    Visibility Visibility,
    TripStatus? Status = null);
