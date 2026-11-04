using System.ComponentModel.DataAnnotations;
using Voyara.Domain.Entities.Guides;
namespace Voyara.Api.DTOs.Guides;

public class CreateGuideRequestDto 
{
    public Guid GuideProfileId { get; set; } 
    public DateOnly Date { get; set; } 
    public TimeOnly StartTime { get; set; } 
    
    [Range(1,int.MaxValue)]
    public int DurationHours { get; set; } 
    
    [Range(1,int.MaxValue)]
    public int PartySize { get; set; } 
    
    [Required, StringLength(10)]
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
    public static GuideSessionReadDto FromEntity(GuideSession s) => new() 
        { 
            Id=s.Id,   
            MeetingLocation=s.MeetingLocation, 
            AgreedPrice=s.AgreedPrice, 
            Currency=s.Currency 
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
    public static GuideRequestReadDto FromEntity(GuideRequest r) => new() 
    { 
        Id=r.Id, 
        UserId=r.UserId, 
        GuideProfileId=r.GuideProfileId, 
        Date=r.Date, 
        StartTime=r.StartTime, 
        DurationHours=r.DurationHours, 
        PartySize=r.PartySize, 
        PreferredLanguage=r.PreferredLanguage, 
        SpecialRequirements=r.SpecialRequirements, 
        Status=r.Status.ToString(), 
        RejectionReason=r.RejectionReason, 
        ExpiresAt=r.ExpiresAt,
        Session=r.GuideSession is null?null:GuideSessionReadDto.FromEntity(r.GuideSession), 
        ChatThreadIds=r.ChatThreads.Select(t=>t.Id).ToList(), 
        Review=r.GuideReview is null?null:GuideReviewReadDto.FromEntity(r.GuideReview) 
    }; 
}
