namespace Voyara.Api.Contracts.Identity;

/// <summary>Request body for ending a session by revoking its refresh token.</summary>
public sealed record LogoutRequest(string RefreshToken);
