using MediatR;
using Voyara.Application.Abstractions;
using Voyara.Application.DTOs.Guides;
using Voyara.Domain.Entities.Guides;
using Voyara.Domain.Repositories.Guides;
using Voyara.SharedKernel.Results;

namespace Voyara.Application.Guides.Commands.UpdateGuideProfile;

public sealed class UpdateGuideProfileCommandHandler : IRequestHandler<UpdateGuideProfileCommand, Result>
{
    private readonly IGuideProfileRepository _profiles;
    private readonly ICurrentUser _currentUser;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateGuideProfileCommandHandler(
        IGuideProfileRepository profiles,
        ICurrentUser currentUser,
        IUnitOfWork unitOfWork)
    {
        _profiles = profiles;
        _currentUser = currentUser;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(UpdateGuideProfileCommand request, CancellationToken cancellationToken)
    {
        if (_currentUser.UserId is not { } userId)
        {
            return Result.Failure(GuideErrors.NotAuthenticated);
        }

        // Looked up by the authenticated user, so a caller can only ever reach their own profile.
        var profile = await _profiles.GetByUserIdWithDetailsAsync(userId, cancellationToken);
        if (profile is null)
        {
            return Result.Failure(GuideErrors.ProfileNotFound);
        }

        var languagesToReAdd = new List<GuideLanguageInput>();

        var update = GuideDomainCall.Execute(() =>
        {
            profile.UpdateDetails(request.Bio, request.PhotoUrl, request.YearsOfExperience);
            languagesToReAdd = SyncLanguages(profile, request.Languages);
            SyncExpertise(profile, request.ExpertiseCategoryIds);
            SyncAvailability(profile, request.Availability);
            SyncPricing(profile, request.Pricing);
        });

        if (update.IsFailure)
        {
            return update;
        }

        if (languagesToReAdd.Count > 0)
        {
            // A language whose level changed was removed (soft-deleted) above. Save that first so the old row is gone from the
            // unique (profile, language) index before the replacement row is inserted. Both saves run inside the command's
            // transaction, so the update is still all-or-nothing.
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var reAdd = GuideDomainCall.Execute(() =>
            {
                foreach (var language in languagesToReAdd)
                    profile.AddLanguage(language.LanguageCode, language.Proficiency);
            });

            if (reAdd.IsFailure)
            {
                return reAdd;
            }
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    /// <returns>The languages whose proficiency changed; they are removed here and must be re-added after a save.</returns>
    private static List<GuideLanguageInput> SyncLanguages(GuideProfile profile, IReadOnlyList<GuideLanguageInput> input)
    {
        var desired = input
            .GroupBy(l => l.LanguageCode.Trim().ToLowerInvariant())
            .ToDictionary(g => g.Key, g => g.First());

        var changed = new List<GuideLanguageInput>();

        foreach (var existing in profile.Languages.ToList())
        {
            if (!desired.TryGetValue(existing.LanguageCode, out var match))
            {
                profile.RemoveLanguage(existing.LanguageCode);
            }
            else if (!string.Equals(match.Proficiency.Trim(), existing.Proficiency, StringComparison.OrdinalIgnoreCase))
            {
                // The domain has no "change level" operation, so a new level is a remove now + add after the save.
                profile.RemoveLanguage(existing.LanguageCode);
                changed.Add(match);
            }
        }

        var current = profile.Languages.Select(l => l.LanguageCode).ToHashSet();
        var pending = changed.Select(l => l.LanguageCode.Trim().ToLowerInvariant()).ToHashSet();
        foreach (var (code, language) in desired)
        {
            if (!current.Contains(code) && !pending.Contains(code))
                profile.AddLanguage(language.LanguageCode, language.Proficiency);
        }

        return changed;
    }

    private static void SyncExpertise(GuideProfile profile, IReadOnlyList<int> categoryIds)
    {
        var desired = categoryIds.ToHashSet();

        foreach (var existing in profile.Expertise.ToList())
        {
            if (!desired.Contains(existing.CategoryId))
                profile.RemoveExpertise(existing.CategoryId);
        }

        var current = profile.Expertise.Select(e => e.CategoryId).ToHashSet();
        foreach (var categoryId in desired.Where(id => !current.Contains(id)))
            profile.AddExpertise(categoryId);
    }

    private static void SyncAvailability(GuideProfile profile, IReadOnlyList<GuideAvailabilityInput> input)
    {
        var desired = input.Distinct().ToList();

        foreach (var existing in profile.Availability.ToList())
        {
            var keep = desired.Any(d =>
                d.Day == existing.Day && d.StartTime == existing.StartTime && d.EndTime == existing.EndTime);
            if (!keep)
                profile.RemoveAvailability(existing.Day, existing.StartTime);
        }

        foreach (var slot in desired)
        {
            var exists = profile.Availability.Any(a =>
                a.Day == slot.Day && a.StartTime == slot.StartTime && a.EndTime == slot.EndTime);
            if (!exists)
                profile.AddAvailability(slot.Day, slot.StartTime, slot.EndTime);
        }
    }

    private static void SyncPricing(GuideProfile profile, IReadOnlyList<GuidePricingInput> input)
    {
        var desired = input
            .GroupBy(p => (p.Unit, Currency: p.Currency.Trim().ToUpperInvariant()))
            .Select(g => g.First())
            .ToList();

        foreach (var existing in profile.Pricing.ToList())
        {
            var keep = desired.Any(d =>
                d.Unit == existing.Unit
                && string.Equals(d.Currency.Trim(), existing.Currency, StringComparison.OrdinalIgnoreCase)
                && d.Amount == existing.Amount);
            if (!keep)
                profile.RemovePricing(existing.Unit, existing.Currency);
        }

        foreach (var price in desired)
        {
            var exists = profile.Pricing.Any(p =>
                p.Unit == price.Unit
                && string.Equals(p.Currency, price.Currency.Trim(), StringComparison.OrdinalIgnoreCase));
            if (!exists)
                profile.AddPricing(price.Unit, price.Amount, price.Currency);
        }
    }
}
