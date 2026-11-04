using MediatR;
using Voyara.Application.Abstractions;
using Voyara.Application.Abstractions.Authorization;
using Voyara.Application.DTOs.Identity;
using Voyara.Domain.Entities.Identity;
using Voyara.Domain.Repositories.Identity;
using Voyara.SharedKernel.Results;
using UserEntity = Voyara.Domain.Entities.Identity.User;

namespace Voyara.Application.Identity.Commands.Tourists.RegisterTourist;

public sealed class RegisterTouristCommandHandler
    : IRequestHandler<RegisterTouristCommand, Result<TouristReadDto>>
{
    private readonly IUserRepository _users;
    private readonly IRoleRepository _roles;
    private readonly ITouristProfileRepository _tourists;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;

    public RegisterTouristCommandHandler(
        IUserRepository users,
        IRoleRepository roles,
        ITouristProfileRepository tourists,
        IUnitOfWork unitOfWork,
        IPasswordHasher passwordHasher)
    {
        _users = users;
        _roles = roles;
        _tourists = tourists;
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
    }

    public async Task<Result<TouristReadDto>> Handle(
        RegisterTouristCommand request,
        CancellationToken cancellationToken)
    {
        if (await _users.EmailExistsAsync(request.Email, cancellationToken))
        {
            return Result.Failure<TouristReadDto>(
                Error.Conflict(
                    "Identity.EmailTaken",
                    $"An account with e-mail '{request.Email.Trim()}' already exists."));
        }

        UserEntity user;
        TouristProfile profile;

        try
        {
            user = UserEntity.Create(
                request.FullName,
                request.Email,
                _passwordHasher.Hash(request.Password));

            profile = TouristProfile.Create(
                user,
                request.NationalityCode,
                request.PreferredLanguage,
                request.Budget,
                request.PhotoUrl);
        }
        catch (InvalidOperationException ex)
        {
            return Result.Failure<TouristReadDto>(
                Error.Conflict("Identity.InvalidState", ex.Message));
        }
        catch (ArgumentException ex)
        {
            return Result.Failure<TouristReadDto>(
                Error.Validation("Identity.InvalidArgument", ex.Message));
        }

        var role = await _roles.GetByNameAsync(Roles.Tourist, cancellationToken);
        if (role is null)
        {
            role = Role.Create(Roles.Tourist);
            _roles.Add(role);
        }

        user.AssignRole(role);

        _users.Add(user);
        _tourists.Add(profile);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return TouristReadDto.FromEntity(profile);
    }
}