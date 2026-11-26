namespace Voyara.Application.DTOs;

/// <summary>Result of a successful registration.</summary>
public sealed record RegisteredUserResponse(Guid Id, string FullName, string Email);
