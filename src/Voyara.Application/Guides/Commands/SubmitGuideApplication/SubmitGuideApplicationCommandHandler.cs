using MediatR;
using Voyara.Application.Abstractions;
using Voyara.Domain.Enums;
using Voyara.Domain.Repositories.Guides;
using Voyara.SharedKernel.Results;

namespace Voyara.Application.Guides.Commands.SubmitGuideApplication;

public sealed class SubmitGuideApplicationCommandHandler : IRequestHandler<SubmitGuideApplicationCommand, Result>
{
    private readonly IGuideProfileRepository _profiles;
    private readonly ICurrentUser _currentUser;

    public SubmitGuideApplicationCommandHandler(IGuideProfileRepository profiles, ICurrentUser currentUser)
    {
        _profiles = profiles;
        _currentUser = currentUser;
    }

    public async Task<Result> Handle(SubmitGuideApplicationCommand request, CancellationToken cancellationToken)
    {
        if (_currentUser.UserId is not { } userId)
        {
            return Result.Failure(GuideErrors.NotAuthenticated);
        }

        var profile = await _profiles.GetByUserIdWithDetailsAsync(userId, cancellationToken);
        if (profile is null)
        {
            return Result.Failure(GuideErrors.ProfileNotFound);
        }

        // GuideProfile has no Rejected -> Pending transition (ST-04), so only pending applications can be confirmed.
        if (profile.Status != GuideStatus.Pending)
        {
            return Result.Failure(GuideErrors.InvalidStatus("submit", profile.Status.ToString()));
        }

        var missing = new List<string>();

        if (profile.Languages.Count == 0)
            missing.Add("at least one language");

        if (profile.Pricing.Count == 0)
            missing.Add("at least one price");

        return missing.Count == 0
            ? Result.Success()
            : Result.Failure(GuideErrors.Incomplete($"The application is missing: {string.Join(", ", missing)}."));
    }
}
