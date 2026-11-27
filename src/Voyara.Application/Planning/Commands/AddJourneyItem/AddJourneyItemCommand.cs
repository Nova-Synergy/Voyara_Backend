using Voyara.Application.Abstractions.Messaging;

namespace Voyara.Application.Planning.Commands.AddJourneyItem;

/// <summary>Command to add a destination stop to a journey itinerary day.</summary>
/// <param name="JourneyId">Identifier of the journey.</param>
/// <param name="DayNumber">One-based itinerary day number.</param>
/// <param name="DestinationId">Identifier of an existing catalog destination.</param>
/// <param name="CityId">Optional GUID city identifier; when supplied, it must match the destination's city.</param>
/// <param name="StartTime">Optional scheduled start time.</param>
/// <param name="EndTime">Optional scheduled end time.</param>
/// <param name="Transport">Optional transportation mode name.</param>
public sealed record AddJourneyItemCommand(
    Guid JourneyId,
    int DayNumber,
    Guid DestinationId,
    Guid? CityId,
    TimeSpan? StartTime,
    TimeSpan? EndTime,
    string? Transport) : ICommand<Guid>;
