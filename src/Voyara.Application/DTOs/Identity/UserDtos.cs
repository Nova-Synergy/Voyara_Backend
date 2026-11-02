using System.ComponentModel.DataAnnotations;
using Voyara.Domain.Entities.Identity;

namespace Voyara.Application.DTOs.Identity;

/// <summary>A user account as returned by the API. Never exposes the password hash or refresh tokens.</summary>
public sealed class UserReadDto
{
    public Guid Id { get; init; }
    public string FullName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public bool EmailConfirmed { get; init; }
    public bool IsActive { get; init; }
    public List<string> Roles { get; init; } = [];
    public DateTimeOffset? CreatedAt { get; init; }

    public static UserReadDto FromEntity(User user) => new()
    {
        Id = user.Id,
        FullName = user.FullName,
        Email = user.Email,
        EmailConfirmed = user.EmailConfirmed,
        IsActive = user.IsActive,
        Roles = user.Roles.Select(r => r.Name).OrderBy(n => n).ToList(),
        CreatedAt = user.CreatedAt
    };
}

/// <summary>Creates a user. Roles default to Tourist when omitted.</summary>
public sealed class CreateUserDto
{
    [Required, StringLength(200)] public string FullName { get; init; } = string.Empty;
    [Required, EmailAddress, StringLength(254)] public string Email { get; init; } = string.Empty;
    [Required, StringLength(128, MinimumLength = 8)] public string Password { get; init; } = string.Empty;
    public List<string>? Roles { get; init; }
}

/// <summary>Updates a user. A null Roles keeps the current roles; a null IsActive keeps the current state.</summary>
public sealed class UpdateUserDto
{
    [Required, StringLength(200)] public string FullName { get; init; } = string.Empty;
    public List<string>? Roles { get; init; }
    public bool? IsActive { get; init; }
}

/// <summary>Creates an administrator account.</summary>
public sealed class CreateAdminDto
{
    [Required, StringLength(200)] public string FullName { get; init; } = string.Empty;
    [Required, EmailAddress, StringLength(254)] public string Email { get; init; } = string.Empty;
    [Required, StringLength(128, MinimumLength = 8)] public string Password { get; init; } = string.Empty;
}

/// <summary>Updates an administrator account. A null IsActive keeps the current state.</summary>
public sealed class UpdateAdminDto
{
    [Required, StringLength(200)] public string FullName { get; init; } = string.Empty;
    public bool? IsActive { get; init; }
}
