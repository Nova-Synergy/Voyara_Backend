namespace Voyara.Api.Contracts.Guides
{
    public sealed record CreateGuideRequestContract(
        Guid GuideProfileId,
        DateOnly Date,
        TimeOnly StartTime,
        int DurationHours,
        int PartySize,
        string PreferredLanguage,
        string? SpecialRequirements);
    
}
