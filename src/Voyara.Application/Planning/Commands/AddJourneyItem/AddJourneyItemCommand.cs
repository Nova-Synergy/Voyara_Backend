using Voyara.Application.Abstractions.Messaging;

namespace Voyara.Application.Planning.Commands.AddJourneyItem;

/// <summary>
/// Command to add a destination stop to a journey itinerary day.
/// </summary>
public sealed record AddJourneyItemCommand(
    Guid JourneyId,
    int DayNumber,
    Guid DestinationId,
    Guid? CityId,
    TimeSpan? StartTime,
    TimeSpan? EndTime,
    string? Transport) : ICommand<Guid>;
