using Voyara.Application.Abstractions;
using Voyara.Application.Abstractions.Authorization;
using Voyara.Application.DTOs.Guides;
using Voyara.Domain.Enums;
using Voyara.SharedKernel.Results;

namespace Voyara.Application.Guides.Queries.GetGuideProfileDetails;

/// <summary>Applies the visibility rules shared by the "by id" and "my profile" queries.</summary>
internal static class GuideDetailsVisibility
{
    public static async Task<Result<GuideProfileDetailsDto>> ApplyAsync(
        GuideDetailsReadModel? model,
        ICurrentUser currentUser,
        IGuideReadRepository readRepository,
        CancellationToken cancellationToken)
    {
        if (model is null)
        {
            return Result.Failure<GuideProfileDetailsDto>(GuideErrors.ProfileNotFound);
        }

        var isOwner = currentUser.IsAuthenticated && currentUser.UserId == model.UserId;
        var isAdmin = currentUser.IsAuthenticated && currentUser.IsInRole(Roles.Admin);
        var canManage = isOwner || isAdmin;

        if (model.Profile.Status != GuideStatus.Approved && !canManage)
        {
            // Same answer as for a missing profile: non-public profiles must not be discoverable.
            return Result.Failure<GuideProfileDetailsDto>(GuideErrors.ProfileNotFound);
        }

        if (!canManage)
        {
            return model.Profile;
        }

        var documents = await readRepository.GetDocumentsAsync(model.Profile.Id, cancellationToken);
        return model.Profile with { Management = new GuideManagementInfoDto(model.RejectionReason, documents) };
    }
}
