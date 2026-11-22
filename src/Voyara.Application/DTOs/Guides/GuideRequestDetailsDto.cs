using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Voyara.Domain.Enums;

namespace Voyara.Application.DTOs.Guides
{
    public sealed record GuideRequestDetailsDto(
        Guid Id,
        Guid GuideProfileId, 
        string GuideName
        , Guid TouristId,
        string TouristName,
        DateOnly Date,
        TimeOnly StartTime,
        int DurationHours,
        int PartySize,
        string PreferredLanguage, 
        string? SpecialRequirements,
        RequestStatus Status,
        string? RejectionReason, 
        DateTimeOffset ExpiresAt,
        DateTimeOffset? CreatedAt,
        GuideSessionDto? Session,
        Guid? ChatThreadId,
        bool HasReview);
}
