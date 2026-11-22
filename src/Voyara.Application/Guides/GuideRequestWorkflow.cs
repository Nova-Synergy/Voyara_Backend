using Voyara.Application.DTOs.Guides;
using Voyara.Domain.Entities.Guides;
using Voyara.Domain.Repositories.Guides;
using Voyara.SharedKernel.Results;

namespace Voyara.Application.Guides;

/// <summary>A request together with the guide profile it is addressed to.</summary>
internal sealed record GuideOwnedRequest(GuideProfile Profile, GuideRequest Request);

/// <summary>Loading and projection steps shared by the request lifecycle handlers.</summary>
internal static class GuideRequestWorkflow
{
    /// <summary>
    /// Resolves the current guide's profile and loads a request addressed to it. Optionally takes the per-guide
    /// transaction lock first, so the request is read after any competing operation has committed.
    /// </summary>
    public static async Task<Result<GuideOwnedRequest>> LoadForGuideAsync(
        Guid requestId,
        Guid guideUserId,
        IGuideProfileRepository profiles,
        IGuideRequestRepository requests,
        bool lockGuide,
        CancellationToken cancellationToken)
    {
        var profile = await profiles.GetByUserIdAsync(guideUserId, cancellationToken);
        if (profile is null)
            return Result.Failure<GuideOwnedRequest>(GuideRequestErrors.GuideProfileRequired);

        if (lockGuide)
            await requests.AcquireGuideLockAsync(profile.Id, cancellationToken);

        var request = await requests.GetByIdAsync(requestId, cancellationToken);
        if (request is null)
            return Result.Failure<GuideOwnedRequest>(GuideRequestErrors.RequestNotFound);

        if (request.GuideProfileId != profile.Id)
            return Result.Failure<GuideOwnedRequest>(GuideRequestErrors.NotAssignedGuide);

        return new GuideOwnedRequest(profile, request);
    }

    /// <summary>Reads the persisted request back as a DTO (after SaveChanges, inside the same transaction).</summary>
    public static async Task<Result<GuideRequestDetailsDto>> ReadAsync(
        IGuideRequestReadRepository readRepository, Guid requestId, CancellationToken cancellationToken)
    {
        var model = await readRepository.GetRequestAsync(requestId, cancellationToken);
        return model is null
            ? Result.Failure<GuideRequestDetailsDto>(GuideRequestErrors.RequestNotFound)
            : model.Request;
    }
}
