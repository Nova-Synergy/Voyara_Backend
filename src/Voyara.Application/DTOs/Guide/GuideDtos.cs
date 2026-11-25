using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Voyara.Domain.Enums;

namespace Voyara.Application.DTOs.Guide
{

    /// <summary>A language spoken by a guide (request input).</summary>
    public sealed record GuideLanguageInput(string LanguageCode, string Proficiency);

    /// <summary>A recurring weekly availability range (request input).</summary>
    public sealed record GuideAvailabilityInput(DayOfWeek Day, TimeOnly StartTime, TimeOnly EndTime);

    /// <summary>A pricing option (request input).</summary>
    public sealed record GuidePricingInput(PricingUnit Unit, decimal Amount, string Currency);

    public sealed record GuideLanguageDto(string LanguageCode, string Proficiency);

    public sealed record GuideAvailabilityDto(DayOfWeek Day, TimeOnly StartTime, TimeOnly EndTime);

    public sealed record GuidePricingDto(PricingUnit Unit, decimal Amount, string Currency);

    /// <summary>Public directory entry. Contains no private or internal data.</summary>
    public sealed record GuideListItemDto(
        Guid Id,
        string FullName,
        string? PhotoUrl,
        int YearsOfExperience,
        decimal AverageRating,
        int ReviewCount,
        Guid CityId,
        IReadOnlyList<string> Languages,
        IReadOnlyList<GuidePricingDto> Pricing);

    /// <summary>Admin queue entry.</summary>
    public sealed record GuideApplicationListItemDto(
        Guid Id,
        Guid UserId,
        string FullName,
        Guid CityId,
        GuideStatus Status,
        DateTimeOffset? SubmittedAt);

    /// <summary>Information visible only to the profile owner and administrators.</summary>
    public sealed record GuideManagementInfoDto(string? RejectionReason);

    /// <summary>Guide profile details. <see cref="Management"/> is null for the general public.</summary>
    public sealed record GuideProfileDetailsDto(
        Guid Id,
        string FullName,
        string? PhotoUrl,
        string Bio,
        int YearsOfExperience,
        decimal AverageRating,
        int ReviewCount,
        Guid CityId,
        GuideStatus Status,
        IReadOnlyList<GuideLanguageDto> Languages,
        IReadOnlyList<int> ExpertiseCategoryIds,
        IReadOnlyList<GuideAvailabilityDto> Availability,
        IReadOnlyList<GuidePricingDto> Pricing,
        GuideManagementInfoDto? Management = null);

}