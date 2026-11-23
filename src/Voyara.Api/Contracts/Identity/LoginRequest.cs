namespace Voyara.Api.Contracts.Identity;

/// <summary>Request body for logging in with e-mail and password.</summary>
public sealed record LoginRequest(string Email, string Password);
