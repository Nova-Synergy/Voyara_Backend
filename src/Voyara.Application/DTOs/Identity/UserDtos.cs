using System.ComponentModel.DataAnnotations;
using Voyara.Domain.Entities.Identity;

namespace Voyara.Application.DTOs.Identity;

public class UserReadDto
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool EmailConfirmed { get; set; }
    public bool IsActive { get; set; }
    public List<string> Roles { get; set; } = [];
    public DateTimeOffset? CreatedAt { get; set; }

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

public class CreateUserDto
{
    [Required, StringLength(200)]
    public string FullName { get; set; } = string.Empty;

    [Required, EmailAddress, StringLength(254)]
    public string Email { get; set; } = string.Empty;

    [Required, StringLength(128, MinimumLength = 8)]
    public string Password { get; set; } = string.Empty;

    public List<string>? Roles { get; set; }
}

public class UpdateUserDto
{
    [Required, StringLength(200)]
    public string FullName { get; set; } = string.Empty;

    public List<string>? Roles { get; set; }

    public bool? IsActive { get; set; }
}

public class CreateAdminDto
{
    [Required, StringLength(200)]
    public string FullName { get; set; } = string.Empty;

    [Required, EmailAddress, StringLength(254)]
    public string Email { get; set; } = string.Empty;

    [Required, StringLength(128, MinimumLength = 8)]
    public string Password { get; set; } = string.Empty;
}

public class UpdateAdminDto
{
    [Required, StringLength(200)]
    public string FullName { get; set; } = string.Empty;

    public bool? IsActive { get; set; }
}