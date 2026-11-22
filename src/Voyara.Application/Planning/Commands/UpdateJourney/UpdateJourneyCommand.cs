using Voyara.Application.Abstractions.Messaging;

namespace Voyara.Application.Planning.Commands.UpdateJourney;

/// <summary>
/// Command to update journey metadata such as title or lifecycle status.
/// </summary>
/// <param name="JourneyId">The unique identifier of the journey.</param>
/// <param name="Title">Optional new title string.</param>
/// <param name="Status">Optional target status string.</param>
public sealed record UpdateJourneyCommand(
    Guid JourneyId,
    string? Title,
    string? Status) : ICommand;
