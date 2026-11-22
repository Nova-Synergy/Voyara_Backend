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

        var update = GuideDomainCall.Execute(() =>
        {
            profile.UpdateDetails(request.Bio, request.PhotoUrl, request.YearsOfExperience);
            SyncLanguages(profile, request.Languages);
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

        foreach (var existing in profile.Languages.ToList())
        {
            if (!desired.TryGetValue(existing.LanguageCode, out var match))
            {
                profile.RemoveLanguage(existing.LanguageCode);
            }
            else if (!string.Equals(match.Proficiency.Trim(), existing.Proficiency, StringComparison.OrdinalIgnoreCase))
            {
                // Changing a level needs remove + add of the same key, which the unique index on
                // (profile, language) does not allow while removed rows are soft-deleted.
                throw new InvalidOperationException(
                    $"Changing the proficiency of language '{existing.LanguageCode}' is not supported yet.");
            }
        }

        var current = profile.Languages.Select(l => l.LanguageCode).ToHashSet();
        foreach (var (code, language) in desired)
        {
            if (!current.Contains(code))
                profile.AddLanguage(language.LanguageCode, language.Proficiency);
        }
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
