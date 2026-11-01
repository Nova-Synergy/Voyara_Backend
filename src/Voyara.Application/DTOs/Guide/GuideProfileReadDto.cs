using Voyara.Domain.Entities.Guides;

namespace Voyara.Api.DTOs.Guides;

/// <summary>Guide profile data returned in lists (no child collections).</summary>
public class GuideProfileReadDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Bio { get; set; } = string.Empty;
    public string? PhotoUrl { get; set; }
    public int YearsOfExperience { get; set; }
    public decimal AverageRating { get; set; }
    public int ReviewCount { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? RejectionReason { get; set; }
    public int CityId { get; set; }

    /// <summary>Manual mapping Entity -> ReadDTO.</summary>
    public static GuideProfileReadDto FromEntity(GuideProfile profile)
    {
        var dto = new GuideProfileReadDto();
        dto.Fill(profile);
        return dto;
    }

    protected void Fill(GuideProfile profile)
    {
        Id = profile.Id;
        UserId = profile.UserId;
        Bio = profile.Bio;
        PhotoUrl = profile.PhotoUrl;
        YearsOfExperience = profile.YearsOfExperience;
        AverageRating = profile.AverageRating;
        ReviewCount = profile.ReviewCount;
        Status = profile.Status.ToString();
        RejectionReason = profile.RejectionReason;
        CityId = profile.CityId;
    }
}
