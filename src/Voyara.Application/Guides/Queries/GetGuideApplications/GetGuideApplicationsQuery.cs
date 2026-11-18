using Voyara.Application.Abstractions.Authorization;
using Voyara.Application.Abstractions.Messaging;
using Voyara.Application.DTOs;
using Voyara.Application.DTOs.Guides;
using Voyara.Domain.Enums;

namespace Voyara.Application.Guides.Queries.GetGuideApplications;

/// <summary>Admin queue of guide profiles, optionally filtered by status (SEQ-13 steps 7–8).</summary>
[AuthorizeRequest(Roles.Admin)]
public sealed record GetGuideApplicationsQuery(GuideStatus? Status, int Page = 1, int PageSize = 20)
    : IQuery<PagedResult<GuideApplicationListItemDto>>;
