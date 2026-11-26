namespace Voyara.Api.Contracts.Identity;

/// <summary>Request body for exchanging a refresh token for a new token pair.</summary>
public sealed record RefreshTokenRequest(string RefreshToken);
