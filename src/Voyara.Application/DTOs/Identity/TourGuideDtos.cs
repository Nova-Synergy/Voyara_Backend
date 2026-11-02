using System.ComponentModel.DataAnnotations;
using Voyara.Domain.Entities.Guides;
using Voyara.Domain.Enums;

namespace Voyara.Application.DTOs.Identity;

/// <summary>A tour guide (user account plus guide profile) as returned by the API.</summary>
public sealed class TourGuideReadDto
{
    public Guid Id { get; init; }
    public Guid UserId { get; init; }
    public string FullName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public int CityId { get; init; }
    public string Bio { get; init; } = string.Empty;
    public string? PhotoUrl { get; init; }
    public int YearsOfExperience { get; init; }
    public decimal AverageRating { get; init; }
    public int ReviewCount { get; init; }
    public GuideStatus Status { get; init; }
    public string? RejectionReason { get; init; }

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

/// <summary>Registers a tour guide: creates the account (TourGuide role) and a profile awaiting approval.</summary>
public sealed class RegisterTourGuideDto
{
    [Required, StringLength(200)] public string FullName { get; init; } = string.Empty;
    [Required, EmailAddress, StringLength(254)] public string Email { get; init; } = string.Empty;
    [Required, StringLength(128, MinimumLength = 8)] public string Password { get; init; } = string.Empty;
    [Range(1, int.MaxValue)] public int CityId { get; init; }
    [Required, StringLength(1000)] public string Bio { get; init; } = string.Empty;
    [StringLength(2048)] public string? PhotoUrl { get; init; }
    [Range(0, 80)] public int YearsOfExperience { get; init; }
}

/// <summary>Updates a tour guide's name and profile details.</summary>
public sealed class UpdateTourGuideDto
{
    [Required, StringLength(200)] public string FullName { get; init; } = string.Empty;
    [Required, StringLength(1000)] public string Bio { get; init; } = string.Empty;
    [StringLength(2048)] public string? PhotoUrl { get; init; }
    [Range(0, 80)] public int YearsOfExperience { get; init; }
}
