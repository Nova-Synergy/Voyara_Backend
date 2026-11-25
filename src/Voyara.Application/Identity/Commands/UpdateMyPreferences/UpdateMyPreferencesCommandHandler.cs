using MediatR;
using Voyara.Application.Abstractions;
using Voyara.Application.DTOs;
using Voyara.Application.Identity.Common;
using Voyara.Application.Mappings;
using Voyara.Domain.Repositories.Identity;
using Voyara.Domain.Repositories.Localization;
using Voyara.SharedKernel.Results;

namespace Voyara.Application.Identity.Commands.UpdateMyPreferences;

/// <summary>Handles replacement of the current tourist's preferences.</summary>
public sealed class UpdateMyPreferencesCommandHandler
    : IRequestHandler<UpdateMyPreferencesCommand, Result<TouristProfileResponse>>
{
    private readonly ITouristProfileRepository _profileRepository;
    private readonly IInterestRepository _interestRepository;
    private readonly ILanguageRepository _languageRepository;
    private readonly ICurrentUser _currentUser;
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>Creates the handler.</summary>
    public UpdateMyPreferencesCommandHandler(
        ITouristProfileRepository profileRepository,
        IInterestRepository interestRepository,
        ILanguageRepository languageRepository,
        ICurrentUser currentUser,
        IUnitOfWork unitOfWork)
    {
        _profileRepository = profileRepository;
        _interestRepository = interestRepository;
        _languageRepository = languageRepository;
        _currentUser = currentUser;
        _unitOfWork = unitOfWork;
    }

    /// <inheritdoc />
    public async Task<Result<TouristProfileResponse>> Handle(
        UpdateMyPreferencesCommand request,
        CancellationToken cancellationToken)
    {
        var loaded = await TouristProfileAccess.LoadCurrentAsync(
            _currentUser, _profileRepository, readOnly: false, cancellationToken);
        if (loaded.IsFailure)
        {
            return Result.Failure<TouristProfileResponse>(loaded.Error);
        }

        var language = request.PreferredLanguage.Trim().ToLowerInvariant();
        if (!await _languageRepository.ExistsByCodeAsync(language, cancellationToken))
        {
            return Result.Failure<TouristProfileResponse>(Error.Validation(
                "TouristProfile.LanguageNotSupported", $"Language '{language}' is not supported."));
        }

        var codes = request.InterestCodes
            .Select(code => code.Trim().ToLowerInvariant())
            .Distinct()
            .ToList();

        var interests = await _interestRepository.GetByCodesAsync(codes, cancellationToken);
        var unknown = codes.Except(interests.Select(interest => interest.Code)).ToList();
        if (unknown.Count > 0)
        {
            return Result.Failure<TouristProfileResponse>(Error.Validation(
                "TouristProfile.InterestNotFound", $"Unknown interests: {string.Join(", ", unknown)}."));
        }

        var profile = loaded.Value;
        profile.ChangePreferredLanguage(language);
        profile.ChangeBudget(request.Budget);

        foreach (var current in profile.Interests.Where(i => !codes.Contains(i.Code)).ToList())
        {
            profile.RemoveInterest(current);
        }

        foreach (var interest in interests)
        {
            profile.AddInterest(interest);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return profile.ToResponse();
    }
}
