using Voyara.Domain.Entities.Guides;
namespace Voyara.Api.DTOs.Guides;
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
    public static GuideProfileReadDto FromEntity(GuideProfile p) => new() 
        {
            Id=p.Id,
            UserId=p.UserId,
            Bio=p.Bio,
            PhotoUrl=p.PhotoUrl,
            YearsOfExperience=p.YearsOfExperience,
            AverageRating=p.AverageRating,
            ReviewCount=p.ReviewCount,
            Status=p.Status.ToString(),
            RejectionReason=p.RejectionReason,
            CityId=p.CityId
        };
}
