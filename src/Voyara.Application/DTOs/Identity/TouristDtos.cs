using System.ComponentModel.DataAnnotations;
using Voyara.Domain.Entities.Identity;
using Voyara.Domain.Enums;

namespace Voyara.Application.DTOs.Identity;

public class TouristReadDto
{
    public Guid UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public string NationalityCode { get; set; } = string.Empty;
    public string PreferredLanguage { get; set; } = string.Empty;
    public BudgetLevel Budget { get; set; }
    public string? PhotoUrl { get; set; }
    public List<string> Interests { get; set; } = [];

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

public class RegisterTouristDto
{
    [Required, StringLength(200)]
    public string FullName { get; set; } = string.Empty;

    [Required, EmailAddress, StringLength(254)]
    public string Email { get; set; } = string.Empty;

    [Required, StringLength(128, MinimumLength = 8)]
    public string Password { get; set; } = string.Empty;

    [Required, StringLength(2, MinimumLength = 2)]
    public string NationalityCode { get; set; } = string.Empty;

    [Required, StringLength(10)]
    public string PreferredLanguage { get; set; } = string.Empty;

    [EnumDataType(typeof(BudgetLevel))]
    public BudgetLevel Budget { get; set; }

    [StringLength(2048)]
    public string? PhotoUrl { get; set; }
}

public class UpdateTouristDto
{
    [Required, StringLength(200)]
    public string FullName { get; set; } = string.Empty;

    [Required, StringLength(2, MinimumLength = 2)]
    public string NationalityCode { get; set; } = string.Empty;

    [Required, StringLength(10)]
    public string PreferredLanguage { get; set; } = string.Empty;

    [EnumDataType(typeof(BudgetLevel))]
    public BudgetLevel Budget { get; set; }

    [StringLength(2048)]
    public string? PhotoUrl { get; set; }
}