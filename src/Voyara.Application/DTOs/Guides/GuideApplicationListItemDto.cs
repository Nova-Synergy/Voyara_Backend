using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Voyara.Domain.Enums;

namespace Voyara.Application.DTOs.Guides
{
    /// <summary>Admin queue entry.</summary>
    public sealed record GuideApplicationListItemDto(
        Guid Id,
        Guid UserId,
        string FullName,
        Guid CityId,
        GuideStatus Status,
        DateTimeOffset? SubmittedAt);
}
