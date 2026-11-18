using Voyara.Application.DTOs.Guides;

namespace Voyara.Api.Contracts.Guides
{
    /// <summary>Body of <c>PUT api/v1/guides/profile</c>; the city cannot be changed.</summary>
    public sealed record UpdateGuideProfileRequest(
    string Bio,
    string? PhotoUrl,
    int YearsOfExperience,
    IReadOnlyList<GuideLanguageInput> Languages,
    IReadOnlyList<int> ExpertiseCategoryIds,
    IReadOnlyList<GuideAvailabilityInput> Availability,
    IReadOnlyList<GuidePricingInput> Pricing);
}
