namespace Voyara.Application.DTOs;

/// <summary>Profile of the authenticated account.</summary>
public sealed record CurrentUserResponse(
    Guid Id,
    string FullName,
    string Email,
    bool EmailConfirmed,
    IReadOnlyList<string> Roles,
    DateTimeOffset? CreatedAt);
