using System.ComponentModel.DataAnnotations;
using Voyara.Domain.Entities.Guides;
using Voyara.Domain.Enums;

namespace Voyara.Application.DTOs.Identity;

public class TourGuideReadDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int CityId { get; set; }
    public string Bio { get; set; } = string.Empty;
    public string? PhotoUrl { get; set; }
    public int YearsOfExperience { get; set; }
    public decimal AverageRating { get; set; }
    public int ReviewCount { get; set; }
    public GuideStatus Status { get; set; }
    public string? RejectionReason { get; set; }

    public static TourGuideReadDto FromEntity(GuideProfile profile) => new()
    {
        Id = profile.Id,
        UserId = profile.UserId,
        FullName = profile.User?.FullName ?? string.Empty,
        Email = profile.User?.Email ?? string.Empty,
        CityId = profile.CityId,
        Bio = profile.Bio,
        PhotoUrl = profile.PhotoUrl,
        YearsOfExperience = profile.YearsOfExperience,
        AverageRating = profile.AverageRating,
        ReviewCount = profile.ReviewCount,
        Status = profile.Status,
        RejectionReason = profile.RejectionReason
    };
}

public class RegisterTourGuideDto
{
    [Required, StringLength(200)]
    public string FullName { get; set; } = string.Empty;

    [Required, EmailAddress, StringLength(254)]
    public string Email { get; set; } = string.Empty;

    [Required, StringLength(128, MinimumLength = 8)]
    public string Password { get; set; } = string.Empty;

    [Range(1, int.MaxValue)]
    public int CityId { get; set; }

    [Required, StringLength(1000)]
    public string Bio { get; set; } = string.Empty;

    [StringLength(2048)]
    public string? PhotoUrl { get; set; }

    [Range(0, 80)]
    public int YearsOfExperience { get; set; }
}

public class UpdateTourGuideDto
{
    [Required, StringLength(200)]
    public string FullName { get; set; } = string.Empty;

    [Required, StringLength(1000)]
    public string Bio { get; set; } = string.Empty;

    [StringLength(2048)]
    public string? PhotoUrl { get; set; }

    [Range(0, 80)]
    public int YearsOfExperience { get; set; }
}