using Voyara.Application.Abstractions.Authorization;
using Voyara.Application.Abstractions.Messaging;
using Voyara.Application.DTOs.Guides;

namespace Voyara.Application.Guides.Commands.CreateGuideReview;

/// <summary>Reviews the guide of a completed request owned by the current tourist.</summary>
[AuthorizeRequest]
public sealed record CreateGuideReviewCommand(Guid GuideRequestId, int Rating, string Text) : ICommand<GuideReviewDto>;
