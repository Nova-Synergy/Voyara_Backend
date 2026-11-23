using Voyara.Application.Abstractions.Messaging;

namespace Voyara.Application.Planning.Commands.CustomizeJourney;

/// <summary>
/// Command to customize an existing journey using free-text instructions processed by the AI travel assistant.
/// </summary>
/// <param name="JourneyId">The unique identifier of the journey to customize.</param>
/// <param name="Instruction">Free-text instructions (e.g. "Focus more on cultural monuments", "Rename to Mediterranean Holiday").</param>
public sealed record CustomizeJourneyCommand(
    Guid JourneyId,
    string Instruction) : ICommand<string>;
