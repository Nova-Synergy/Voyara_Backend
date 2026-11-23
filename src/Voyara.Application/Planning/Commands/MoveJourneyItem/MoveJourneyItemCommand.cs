using Voyara.Application.Abstractions.Messaging;

namespace Voyara.Application.Planning.Commands.MoveJourneyItem;

/// <summary>
/// Command to move and resequence an itinerary stop item across days within a journey.
/// </summary>
/// <param name="JourneyId">The unique identifier of the owning journey.</param>
/// <param name="ItemId">The unique identifier of the itinerary item to move.</param>
/// <param name="TargetDayNumber">The destination day number (1-based).</param>
/// <param name="NewOrder">The target sequence order index within that day (1-based).</param>
public sealed record MoveJourneyItemCommand(
    Guid JourneyId,
    Guid ItemId,
    int TargetDayNumber,
    int NewOrder) : ICommand;
