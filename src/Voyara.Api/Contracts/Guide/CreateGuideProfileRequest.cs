namespace Voyara.Api.Contracts.Guide
{
    /// <summary>Body of <c>POST api/v1/guides/profile</c>. The owner is the authenticated user.</summary>
    public sealed record CreateGuideProfileRequest(
        Guid CityId,
        string Bio,
        string? PhotoUrl,
        int YearsOfExperience,
        IReadOnlyList<GuideLanguageInput> Languages,
        IReadOnlyList<int> ExpertiseCategoryIds,
        IReadOnlyList<GuideAvailabilityInput> Availability,
        IReadOnlyList<GuidePricingInput> Pricing
        );
}
