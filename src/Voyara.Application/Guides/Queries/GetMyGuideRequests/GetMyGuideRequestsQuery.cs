using Voyara.Application.Abstractions.Authorization;
using Voyara.Application.Abstractions.Messaging;
using Voyara.Application.DTOs;
using Voyara.Application.DTOs.Guides;
using Voyara.Domain.Enums;

namespace Voyara.Application.Guides.Queries.GetMyGuideRequests;

/// <summary>Lists the requests created by the current tourist.</summary>
[AuthorizeRequest(Roles.Tourist)]
public sealed record GetMyGuideRequestsQuery(RequestStatus? Status, int Page = 1, int PageSize = 10)
    : IQuery<PagedResult<GuideRequestListItemDto>>;
