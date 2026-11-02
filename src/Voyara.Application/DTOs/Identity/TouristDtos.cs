using System.ComponentModel.DataAnnotations;
using Voyara.Domain.Entities.Identity;
using Voyara.Domain.Enums;

namespace Voyara.Application.DTOs.Identity;

/// <summary>A tourist (user account plus tourist profile) as returned by the API.</summary>
public sealed class TouristReadDto
{
    public Guid UserId { get; init; }
    public string FullName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public bool IsActive { get; init; }
    public string NationalityCode { get; init; } = string.Empty;
    public string PreferredLanguage { get; init; } = string.Empty;
    public BudgetLevel Budget { get; init; }
    public string? PhotoUrl { get; init; }
    public List<string> Interests { get; init; } = [];

    public static TouristReadDto FromEntity(TouristProfile profile) => new()
    {
        UserId = profile.UserId,
        FullName = profile.User.FullName,
        Email = profile.User.Email,
        IsActive = profile.User.IsActive,
        NationalityCode = profile.NationalityCode,
        PreferredLanguage = profile.PreferredLanguage,
        Budget = profile.Budget,
        PhotoUrl = profile.PhotoUrl,
        Interests = profile.Interests.Select(i => i.Code).OrderBy(c => c).ToList()
    };
}

/// <summary>Registers a tourist: creates the account (Tourist role) and the profile.</summary>
public sealed class RegisterTouristDto
{
    [Required, StringLength(200)] public string FullName { get; init; } = string.Empty;
    [Required, EmailAddress, StringLength(254)] public string Email { get; init; } = string.Empty;
    [Required, StringLength(128, MinimumLength = 8)] public string Password { get; init; } = string.Empty;
    [Required, StringLength(2, MinimumLength = 2)] public string NationalityCode { get; init; } = string.Empty;
    [Required, StringLength(10)] public string PreferredLanguage { get; init; } = string.Empty;
    [EnumDataType(typeof(BudgetLevel))] public BudgetLevel Budget { get; init; }
    [StringLength(2048)] public string? PhotoUrl { get; init; }
}

/// <summary>Updates a tourist's name and profile.</summary>
public sealed class UpdateTouristDto
{
    [Required, StringLength(200)] public string FullName { get; init; } = string.Empty;
    [Required, StringLength(2, MinimumLength = 2)] public string NationalityCode { get; init; } = string.Empty;
    [Required, StringLength(10)] public string PreferredLanguage { get; init; } = string.Empty;
    [EnumDataType(typeof(BudgetLevel))] public BudgetLevel Budget { get; init; }
    [StringLength(2048)] public string? PhotoUrl { get; init; }
}
