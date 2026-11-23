namespace Voyara.Api.Contracts.Identity;

/// <summary>Request body for registering a new tourist account.</summary>
public sealed record RegisterRequest(string FullName, string Email, string Password);
