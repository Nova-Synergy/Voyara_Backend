using Voyara.Domain.Enums;

namespace Voyara.Api.Contracts.Planning;

/// <summary>
/// Request contract for creating a new tourist trip plan.
/// </summary>
/// <param name="Title">Descriptive title of the trip.</param>
/// <param name="StartDate">Planned arrival/start date.</param>
/// <param name="EndDate">Planned departure/end date.</param>
/// <param name="Visibility">Initial privacy level (defaults to Private).</param>
/// <param name="JourneyId">Optional identifier of an existing journey to link.</param>
public sealed record CreateTripRequest(
    string Title,
    DateOnly StartDate,
    DateOnly EndDate,
    Visibility Visibility = Visibility.Private,
    Guid? JourneyId = null);
