using MediatR;
using Voyara.Application.Abstractions;
using Voyara.Application.Abstractions.Authorization;
using Voyara.Application.DTOs.Identity;
using Voyara.Domain.Entities.Guides;
using Voyara.Domain.Entities.Identity;
using Voyara.Domain.Repositories.Geography;
using Voyara.Domain.Repositories.Guides;
using Voyara.Domain.Repositories.Identity;
using Voyara.SharedKernel.Results;
using UserEntity = Voyara.Domain.Entities.Identity.User;

namespace Voyara.Application.Identity.Commands.TourGuides.RegisterTourGuide;

public sealed class RegisterTourGuideCommandHandler
    : IRequestHandler<RegisterTourGuideCommand, Result<TourGuideReadDto>>
{
    private readonly IUserRepository _users;
    private readonly IRoleRepository _roles;
    private readonly IGuideProfileRepository _guides;
    private readonly ICityRepository _cities;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;

    public RegisterTourGuideCommandHandler(
        IUserRepository users,
        IRoleRepository roles,
        IGuideProfileRepository guides,
        ICityRepository cities,
        IUnitOfWork unitOfWork,
        IPasswordHasher passwordHasher)
    {
        _users = users;
        _roles = roles;
        _guides = guides;
        _cities = cities;
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
    }

    public async Task<Result<TourGuideReadDto>> Handle(
        RegisterTourGuideCommand request,
        CancellationToken cancellationToken)
    {
        if (!await _cities.ActiveCityExistsAsync(request.CityId, cancellationToken))
        {
            return Result.Failure<TourGuideReadDto>(
                Error.Validation(
                    "Identity.CityNotFound",
                    $"City '{request.CityId}' was not found."));
        }

        if (await _users.EmailExistsAsync(request.Email, cancellationToken))
        {
            return Result.Failure<TourGuideReadDto>(
                Error.Conflict(
                    "Identity.EmailTaken",
                    $"An account with e-mail '{request.Email.Trim()}' already exists."));
        }

        UserEntity user;
        GuideProfile profile;

        try
        {
            user = UserEntity.Create(
                request.FullName,
                request.Email,
                _passwordHasher.Hash(request.Password));

            profile = GuideProfile.Create(
                user.Id,
                request.CityId,
                request.Bio,
                request.PhotoUrl,
                request.YearsOfExperience);
        }
        catch (InvalidOperationException ex)
        {
            return Result.Failure<TourGuideReadDto>(
                Error.Conflict("Identity.InvalidState", ex.Message));
        }
        catch (ArgumentException ex)
        {
            return Result.Failure<TourGuideReadDto>(
                Error.Validation("Identity.InvalidArgument", ex.Message));
        }

        var role = await _roles.GetByNameAsync(Roles.TourGuide, cancellationToken);
        if (role is null)
        {
            role = Role.Create(Roles.TourGuide);
            _roles.Add(role);
        }

        user.AssignRole(role);

        _users.Add(user);
        _guides.Add(profile);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return TourGuideReadDto.FromEntity(profile);
    }
}