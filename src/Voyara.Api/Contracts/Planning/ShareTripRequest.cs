namespace Voyara.Api.Contracts.Planning;

/// <summary>
/// Request contract for generating a secure shareable token for a trip.
/// </summary>
/// <param name="ExpiresAt">Optional expiration timestamp in UTC.</param>
public sealed record ShareTripRequest(DateTime? ExpiresAt = null);
