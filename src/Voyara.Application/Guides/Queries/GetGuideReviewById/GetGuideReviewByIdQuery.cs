using Voyara.Application.Abstractions.Messaging;
using Voyara.Application.DTOs.Guides;

namespace Voyara.Application.Guides.Queries.GetGuideReviewById;

/// <summary>Gets one review, subject to the same visibility as the guide's review list.</summary>
public sealed record GetGuideReviewByIdQuery(Guid ReviewId) : IQuery<GuideReviewDto>;
