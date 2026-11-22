using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Voyara.Domain.Enums;

namespace Voyara.Application.DTOs.Guides
{

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

}
