using Voyara.Application.Abstractions.Authorization;
using Voyara.Application.Abstractions.Messaging;
using Voyara.Application.DTOs.Guides;

namespace Voyara.Application.Guides.Queries.GetGuideRequestById;

/// <summary>Gets one request; allowed for the tourist who created it and the guide it is addressed to.</summary>
[AuthorizeRequest]
public sealed record GetGuideRequestByIdQuery(Guid RequestId) : IQuery<GuideRequestDetailsDto>;
