namespace Voyara.Api.Contracts.Planning;

/// <summary>
/// HTTP request contract for moving a journey stop to another day or sequence position.
/// </summary>
/// <param name="TargetDayNumber">The 1-based target day number.</param>
/// <param name="NewOrder">The desired 1-based sort order within the target day.</param>
public sealed record MoveJourneyItemRequest(int TargetDayNumber, int NewOrder);
