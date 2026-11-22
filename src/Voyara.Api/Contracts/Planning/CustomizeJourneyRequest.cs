namespace Voyara.Api.Contracts.Planning;

/// <summary>
/// HTTP request contract for natural language journey customization.
/// </summary>
/// <param name="Instruction">Free-text instructions or change requests for the journey.</param>
public sealed record CustomizeJourneyRequest(string Instruction);
