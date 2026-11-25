using MediatR;
using Voyara.Application.Abstractions;
using Voyara.Domain.Entities.Guides;
using Voyara.Domain.Repositories.Guides;
using Voyara.SharedKernel.Results;

namespace Voyara.Application.Guides.Commands.CreateGuideProfile;

public sealed class CreateGuideProfileCommandHandler : IRequestHandler<CreateGuideProfileCommand, Result<Guid>>
{
    private readonly IGuideProfileRepository _profiles;
    private readonly ICurrentUser _currentUser;
    private readonly IUnitOfWork _unitOfWork;

    public CreateGuideProfileCommandHandler(
        IGuideProfileRepository profiles,
        ICurrentUser currentUser,
        IUnitOfWork unitOfWork)
    {
        _profiles = profiles;
        _currentUser = currentUser;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(CreateGuideProfileCommand request, CancellationToken cancellationToken)
    {
        if (_currentUser.UserId is not { } userId)
        {
            return Result.Failure<Guid>(GuideErrors.NotAuthenticated);
        }

        if (await _profiles.ExistsByUserIdAsync(userId, cancellationToken))
        {
            return Result.Failure<Guid>(GuideErrors.ProfileAlreadyExists);
        }

        GuideProfile? profile = null;

        var build = GuideDomainCall.Execute(() =>
        {
            var created = GuideProfile.Create(
                userId, request.CityId, request.Bio, request.PhotoUrl, request.YearsOfExperience);

            foreach (var language in request.Languages)
                created.AddLanguage(language.LanguageCode, language.Proficiency);

            foreach (var categoryId in request.ExpertiseCategoryIds.Distinct())
                created.AddExpertise(categoryId);

            foreach (var slot in request.Availability)
                created.AddAvailability(slot.Day, slot.StartTime, slot.EndTime);

            foreach (var price in request.Pricing)
                created.AddPricing(price.Unit, price.Amount, price.Currency);

            profile = created;
        });

        if (build.IsFailure)
        {
            return Result.Failure<Guid>(build.Error);
        }

        _profiles.Add(profile!);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(profile!.Id);
    }
}
