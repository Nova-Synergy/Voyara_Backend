using Voyara.Application.Abstractions;
using Voyara.Application.Abstractions.Authorization;
using Voyara.Domain.Enums;

namespace Voyara.Application.Guides.Queries.GetGuideReviews;

/// <summary>Same visibility policy as guide profile details: non-public profiles look "not found" to everyone but owner/admin.</summary>
internal static class GuideReviewVisibility
{
    public static bool IsVisible(GuideReviewTarget? target, ICurrentUser currentUser)
    {
        if (target is null)
            return false;

        if (target.Status == GuideStatus.Approved)
            return true;

        return currentUser.IsAuthenticated
            && (currentUser.UserId == target.GuideUserId || currentUser.IsInRole(Roles.Admin));
    }
}
