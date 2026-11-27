using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Voyara.Domain.Enums;

namespace Voyara.Application.DTOs.Guides
{
    /// <summary>Summary of a guide request for list views.</summary>
    public sealed record GuideRequestListItemDto(
        Guid Id,
        Guid GuideProfileId,
        string GuideName,
        string TouristName,
        DateOnly Date,
        TimeOnly StartTime,
        int DurationHours,
        int PartySize,
        string PreferredLanguage,
        RequestStatus Status,
        DateTimeOffset ExpiresAt,
        DateTimeOffset? CreatedAt);

    /// <summary>Agreed session created when a guide accepts a request.</summary>
    public sealed record GuideSessionDto(
        Guid Id,
        Guid GuideRequestId,
        string MeetingLocation,
        decimal AgreedPrice,
        string Currency);

    /// <summary>Full details of a guide request.</summary>
    public sealed record GuideRequestDetailsDto(
        Guid Id,
        Guid GuideProfileId,
        string GuideName,
        Guid TouristId,
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

    /// <summary>A chat message.</summary>
    public sealed record ChatMessageDto(
        Guid Id,
        Guid ChatThreadId,
        Guid SenderId,
        string Text,
        DateTime SentAt,
        DateTime? ReadAt);

    /// <summary>A guide review.</summary>
    public sealed record GuideReviewDto(
        Guid Id,
        Guid GuideRequestId,
        Guid GuideProfileId,
        string ReviewerName,
        int Rating,
        string Text,
        DateTimeOffset? CreatedAt);

}
