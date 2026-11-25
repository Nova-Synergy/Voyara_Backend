using MediatR;
using Voyara.Application.Abstractions;
using Voyara.Application.DTOs.Guide;
using Voyara.Domain.Entities.Guides;
using Voyara.Domain.Enums;
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

        var update = GuideDomainCall.Execute(() =>
        {
            // Languages first: it can reject the request, and nothing may be changed before that happens.
            SyncLanguages(profile, request.Languages);
            profile.UpdateDetails(request.Bio, request.PhotoUrl, request.YearsOfExperience);
            SyncExpertise(profile, request.ExpertiseCategoryIds);
            SyncAvailability(profile, request.Availability);
            SyncPricing(profile, request.Pricing);
        });

        if (update.IsFailure)
        {
            return update;
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    private static void SyncLanguages(GuideProfile profile, IReadOnlyList<GuideLanguageInput> input)
    {
        var desired = input
            .GroupBy(l => l.LanguageCode.Trim().ToLowerInvariant())
            .ToDictionary(g => g.Key, g => g.First());

        // Decide everything from the state before any change, then apply.
        var toRemove = new List<string>();
        var toUpdate = new List<GuideLanguageInput>();
        var existingCodes = new HashSet<string>();

        foreach (var existing in profile.Languages)
        {
            existingCodes.Add(existing.LanguageCode);

            if (!desired.TryGetValue(existing.LanguageCode, out var match))
            {
                toRemove.Add(existing.LanguageCode);
            }
            else if (!string.Equals(match.Proficiency.Trim(), existing.Proficiency, StringComparison.Ordinal))
            {
                toUpdate.Add(match);
            }
        }

        if (toUpdate.Count > 0)
        {
            // GuideProfile cannot change a proficiency in place, and remove + add of the same language
            // would hit the unique index because removed rows are soft-deleted.
            throw new InvalidOperationException(
                $"Changing the proficiency of an existing language is not supported yet ({string.Join(", ", toUpdate.Select(l => l.LanguageCode))}).");
        }

        var toAdd = desired.Where(kv => !existingCodes.Contains(kv.Key)).Select(kv => kv.Value).ToList();

        foreach (var code in toRemove)
            profile.RemoveLanguage(code);

        foreach (var language in toAdd)
            profile.AddLanguage(language.LanguageCode, language.Proficiency);
    }

    private static void SyncExpertise(GuideProfile profile, IReadOnlyList<int> categoryIds)
    {
        var desired = categoryIds.ToHashSet();
        var existingIds = profile.Expertise.Select(e => e.CategoryId).ToHashSet();

        var toRemove = existingIds.Where(id => !desired.Contains(id)).ToList();
        var toAdd = desired.Where(id => !existingIds.Contains(id)).ToList();

        foreach (var categoryId in toRemove)
            profile.RemoveExpertise(categoryId);

        foreach (var categoryId in toAdd)
            profile.AddExpertise(categoryId);
    }

    private static void SyncAvailability(GuideProfile profile, IReadOnlyList<GuideAvailabilityInput> input)
    {
        var desired = input.Distinct().ToList();
        var existing = profile.Availability.ToList();

        var toRemove = existing
            .Where(e => !desired.Any(d => d.Day == e.Day && d.StartTime == e.StartTime && d.EndTime == e.EndTime))
            .ToList();

        var toAdd = desired
            .Where(d => !existing.Any(e => e.Day == d.Day && e.StartTime == d.StartTime && e.EndTime == d.EndTime))
            .ToList();

        // Removals first so a replaced range does not trip the domain's overlap check.
        foreach (var slot in toRemove)
            profile.RemoveAvailability(slot.Day, slot.StartTime);

        foreach (var slot in toAdd)
            profile.AddAvailability(slot.Day, slot.StartTime, slot.EndTime);
    }

    private static void SyncPricing(GuideProfile profile, IReadOnlyList<GuidePricingInput> input)
    {
        var desired = input
            .GroupBy(p => (p.Unit, Currency: p.Currency.Trim().ToUpperInvariant()))
            .Select(g => g.First())
            .ToList();
        var existing = profile.Pricing.ToList();

        // A price is "unchanged" only when unit, currency AND amount all match. A changed amount therefore
        // appears in both lists: the old row is removed and the replacement is always added. Both lists are
        // computed from the state before any mutation, so the add never depends on what the removal did.
        var toRemove = existing
            .Where(e => !desired.Any(d => Matches(d, e.Unit, e.Currency) && d.Amount == e.Amount))
            .ToList();

        var toAdd = desired
            .Where(d => !existing.Any(e => Matches(d, e.Unit, e.Currency) && d.Amount == e.Amount))
            .ToList();

        foreach (var price in toRemove)
            profile.RemovePricing(price.Unit, price.Currency);

        foreach (var price in toAdd)
            profile.AddPricing(price.Unit, price.Amount, price.Currency);
    }

    private static bool Matches(GuidePricingInput input, PricingUnit unit, string currency) =>
        input.Unit == unit
        && string.Equals(input.Currency.Trim(), currency, StringComparison.OrdinalIgnoreCase);
}
