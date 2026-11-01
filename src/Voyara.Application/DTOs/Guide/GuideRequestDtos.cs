using System.ComponentModel.DataAnnotations;
using Voyara.Domain.Entities.Guides;

namespace Voyara.Api.DTOs.Guides;

public class CreateGuideRequestDto
{
    public Guid GuideProfileId { get; set; }

    public DateOnly Date { get; set; }

    public TimeOnly StartTime { get; set; }

    [Range(1, int.MaxValue)]
    public int DurationHours { get; set; }

    [Range(1, int.MaxValue)]
    public int PartySize { get; set; }

    [Required]
    [StringLength(10)]
    public string PreferredLanguage { get; set; } = string.Empty;

    [StringLength(1000)]
    public string? SpecialRequirements { get; set; }
}

public class GuideSessionReadDto
{
    public Guid Id { get; set; }
    public string MeetingLocation { get; set; } = string.Empty;
    public decimal AgreedPrice { get; set; }
    public string Currency { get; set; } = string.Empty;

    public static GuideSessionReadDto FromEntity(GuideSession session) => new()
    {
        Id = session.Id,
        MeetingLocation = session.MeetingLocation,
        AgreedPrice = session.AgreedPrice,
        Currency = session.Currency
    };
}

public class GuideRequestReadDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid GuideProfileId { get; set; }
    public DateOnly Date { get; set; }
    public TimeOnly StartTime { get; set; }
    public int DurationHours { get; set; }
    public int PartySize { get; set; }
    public string PreferredLanguage { get; set; } = string.Empty;
    public string? SpecialRequirements { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? RejectionReason { get; set; }
    public DateTimeOffset ExpiresAt { get; set; }
    public GuideSessionReadDto? Session { get; set; }
    public List<Guid> ChatThreadIds { get; set; } = new();
    public GuideReviewReadDto? Review { get; set; }

    /// <summary>Manual mapping Entity -> ReadDTO. Session, threads and review appear when they are loaded.</summary>
    public static GuideRequestReadDto FromEntity(GuideRequest request) => new()
    {
        Id = request.Id,
        UserId = request.UserId,
        GuideProfileId = request.GuideProfileId,
        Date = request.Date,
        StartTime = request.StartTime,
        DurationHours = request.DurationHours,
        PartySize = request.PartySize,
        PreferredLanguage = request.PreferredLanguage,
        SpecialRequirements = request.SpecialRequirements,
        Status = request.Status.ToString(),
        RejectionReason = request.RejectionReason,
        ExpiresAt = request.ExpiresAt,
        Session = request.GuideSession is null ? null : GuideSessionReadDto.FromEntity(request.GuideSession),
        ChatThreadIds = request.ChatThreads.Select(thread => thread.Id).ToList(),
        Review = request.GuideReview is null ? null : GuideReviewReadDto.FromEntity(request.GuideReview)
    };
}
