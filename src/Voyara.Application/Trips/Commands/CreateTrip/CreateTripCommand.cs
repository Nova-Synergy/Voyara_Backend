using Voyara.Application.Abstractions.Messaging;
using Voyara.Domain.Enums;

namespace Voyara.Application.Trips.Commands.CreateTrip;

/// <summary>
/// Command to create a new trip aggregate and return its identifier.
/// </summary>
/// <param name="Title">Descriptive title of the trip.</param>
/// <param name="StartDate">Planned departure date.</param>
/// <param name="EndDate">Planned return date.</param>
/// <param name="Visibility">Initial privacy level.</param>
/// <param name="JourneyId">Optional associated journey itinerary identifier.</param>
public sealed record CreateTripCommand(
    string Title,
    DateOnly StartDate,
    DateOnly EndDate,
    Visibility Visibility = Visibility.Private,
    Guid? JourneyId = null) : ICommand<Guid>;
