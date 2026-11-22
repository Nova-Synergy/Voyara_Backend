using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Voyara.Domain.Enums;

namespace Voyara.Application.DTOs.Guides
{
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
