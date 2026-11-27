using Voyara.Application.Abstractions.Authorization;
using Voyara.Application.Abstractions.Messaging;
using Voyara.Application.DTOs;
using Voyara.Application.DTOs.Guides;
using Voyara.Domain.Enums;

namespace Voyara.Application.Guides.Queries.GetIncomingGuideRequests;

/// <summary>Lists the requests addressed to the current guide.</summary>
[AuthorizeRequest(Roles.TourGuide)]
public sealed record GetIncomingGuideRequestsQuery(RequestStatus? Status, int Page = 1, int PageSize = 10)
    : IQuery<PagedResult<GuideRequestListItemDto>>;
