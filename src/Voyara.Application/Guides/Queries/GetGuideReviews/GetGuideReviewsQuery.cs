using Voyara.Application.Abstractions.Messaging;
using Voyara.Application.DTOs;
using Voyara.Application.DTOs.Guides;

namespace Voyara.Application.Guides.Queries.GetGuideReviews;

/// <summary>Lists a guide's reviews. Public for approved guides; other profiles are visible to their owner and admins only.</summary>
public sealed record GetGuideReviewsQuery(Guid GuideProfileId, int Page = 1, int PageSize = 10)
    : IQuery<PagedResult<GuideReviewDto>>;
