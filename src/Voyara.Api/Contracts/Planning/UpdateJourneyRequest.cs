namespace Voyara.Api.Contracts.Planning;

/// <summary>
/// HTTP request contract for updating journey title or status.
/// </summary>
/// <param name="Title">Optional new title string.</param>
/// <param name="Status">Optional new status string (e.g. "Draft", "Confirmed", "Cancelled").</param>
public sealed record UpdateJourneyRequest(string? Title, string? Status);
