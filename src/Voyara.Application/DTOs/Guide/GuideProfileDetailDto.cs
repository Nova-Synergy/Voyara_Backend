using Voyara.Domain.Entities.Guides;

namespace Voyara.Api.DTOs.Guides;

/// <summary>Guide profile with its child collections (returned when one profile is requested).</summary>
public class GuideProfileDetailDto : GuideProfileReadDto
{
    public List<GuideLanguageReadDto> Languages { get; set; } = new();
    public List<GuideExpertiseReadDto> Expertise { get; set; } = new();
    public List<GuideAvailabilityReadDto> Availability { get; set; } = new();
    public List<GuideTimeOffReadDto> TimeOff { get; set; } = new();
    public List<GuidePricingReadDto> Pricing { get; set; } = new();

    /// <summary>Manual mapping Entity -> DetailDTO. The profile must be loaded with its children.</summary>
    public static new GuideProfileDetailDto FromEntity(GuideProfile profile)
    {
        var dto = new GuideProfileDetailDto();
        dto.Fill(profile);
        dto.Languages = profile.Languages.Select(GuideLanguageReadDto.FromEntity).ToList();
        dto.Expertise = profile.Expertise.Select(GuideExpertiseReadDto.FromEntity).ToList();
        dto.Availability = profile.Availability.Select(GuideAvailabilityReadDto.FromEntity).ToList();
        dto.TimeOff = profile.TimeOff.Select(GuideTimeOffReadDto.FromEntity).ToList();
        dto.Pricing = profile.Pricing.Select(GuidePricingReadDto.FromEntity).ToList();
        return dto;
    }
}
