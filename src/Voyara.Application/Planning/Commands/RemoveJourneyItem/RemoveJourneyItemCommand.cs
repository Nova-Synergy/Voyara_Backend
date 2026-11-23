using Voyara.Application.Abstractions.Messaging;

namespace Voyara.Application.Planning.Commands.RemoveJourneyItem;

/// <summary>
/// Command to remove a destination stop from a journey itinerary.
/// </summary>
/// <param name="JourneyId">The unique identifier of the journey.</param>
/// <param name="ItemId">The unique identifier of the stop item to remove.</param>
public sealed record RemoveJourneyItemCommand(Guid JourneyId, Guid ItemId) : ICommand;
