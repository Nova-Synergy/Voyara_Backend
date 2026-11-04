using MediatR;
using Voyara.Application.Abstractions;
using Voyara.Application.Abstractions.Authorization;
using Voyara.Application.DTOs.Identity;
using Voyara.Domain.Entities.Identity;
using Voyara.Domain.Repositories.Identity;
using Voyara.SharedKernel.Results;
using UserEntity = Voyara.Domain.Entities.Identity.User;

namespace Voyara.Application.Identity.Commands.Admins.CreateAdmin;

public sealed class CreateAdminCommandHandler
    : IRequestHandler<CreateAdminCommand, Result<UserReadDto>>
{
    private readonly IUserRepository _users;
    private readonly IRoleRepository _roles;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;

    public CreateAdminCommandHandler(
        IUserRepository users,
        IRoleRepository roles,
        IUnitOfWork unitOfWork,
        IPasswordHasher passwordHasher)
    {
        _users = users;
        _roles = roles;
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
    }

    public async Task<Result<UserReadDto>> Handle(
        CreateAdminCommand request,
        CancellationToken cancellationToken)
    {
        if (await _users.EmailExistsAsync(request.Email, cancellationToken))
        {
            return Result.Failure<UserReadDto>(
                Error.Conflict(
                    "Identity.EmailTaken",
                    $"An account with e-mail '{request.Email.Trim()}' already exists."));
        }

        UserEntity admin;

        try
        {
            admin = UserEntity.Create(
                request.FullName,
                request.Email,
                _passwordHasher.Hash(request.Password));
        }
        catch (InvalidOperationException ex)
        {
            return Result.Failure<UserReadDto>(
                Error.Conflict("Identity.InvalidState", ex.Message));
        }
        catch (ArgumentException ex)
        {
            return Result.Failure<UserReadDto>(
                Error.Validation("Identity.InvalidArgument", ex.Message));
        }

        var role = await _roles.GetByNameAsync(Roles.Admin, cancellationToken);
        if (role is null)
        {
            role = Role.Create(Roles.Admin);
            _roles.Add(role);
        }

        admin.AssignRole(role);

        _users.Add(admin);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return UserReadDto.FromEntity(admin);
    }
}