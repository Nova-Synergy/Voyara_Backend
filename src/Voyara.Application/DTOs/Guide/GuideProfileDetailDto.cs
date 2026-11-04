using Voyara.Domain.Entities.Guides;
namespace Voyara.Api.DTOs.Guides;

public class GuideProfileDetailDto : GuideProfileReadDto 
{
    public List<GuideLanguageReadDto> Languages { get; set; } = new(); 
    public List<GuideExpertiseReadDto> Expertise { get; set; } = new(); 
    public List<GuideAvailabilityReadDto> Availability { get; set; } = new(); 
    public List<GuideTimeOffReadDto> TimeOff { get; set; } = new(); 
    public List<GuidePricingReadDto> Pricing { get; set; } = new();
    public static new GuideProfileDetailDto FromEntity(GuideProfile p) => new() 
        {   Id=p.Id,
            UserId=p.UserId,
            Bio=p.Bio,
            PhotoUrl=p.PhotoUrl,
            YearsOfExperience=p.YearsOfExperience,
            AverageRating=p.AverageRating,
            ReviewCount=p.ReviewCount,
            Status=p.Status.ToString(),
            RejectionReason=p.RejectionReason,
            CityId=p.CityId,
            Languages=p.Languages.Select(GuideLanguageReadDto.FromEntity).ToList(),
            Expertise=p.Expertise.Select(GuideExpertiseReadDto.FromEntity).ToList(),
            Availability=p.Availability.Select(GuideAvailabilityReadDto.FromEntity).ToList(),
            TimeOff=p.TimeOff.Select(GuideTimeOffReadDto.FromEntity).ToList(),
            Pricing=p.Pricing.Select(GuidePricingReadDto.FromEntity).ToList()
        };
}
