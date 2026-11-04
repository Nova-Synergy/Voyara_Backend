using MediatR;
using Voyara.Application.Abstractions;
using Voyara.Application.Abstractions.Authorization;
using Voyara.Application.DTOs.Identity;
using Voyara.Domain.Entities.Identity;
using Voyara.Domain.Repositories.Identity;
using Voyara.SharedKernel.Results;
using UserEntity = Voyara.Domain.Entities.Identity.User;

namespace Voyara.Application.Identity.Commands.Users.CreateUser;

public sealed class CreateUserCommandHandler
    : IRequestHandler<CreateUserCommand, Result<UserReadDto>>
{
    private static readonly string[] AllowedRoles =
        [Roles.Tourist, Roles.TourGuide, Roles.Admin];

    private readonly IUserRepository _users;
    private readonly IRoleRepository _roles;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;

    public CreateUserCommandHandler(
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
        CreateUserCommand request,
        CancellationToken cancellationToken)
    {
        var roles = new List<string> { Roles.Tourist };

        if (request.Roles is { Count: > 0 })
        {
            roles.Clear();

            foreach (var name in request.Roles)
            {
                var role = AllowedRoles.FirstOrDefault(
                    allowed => string.Equals(
                        allowed,
                        name?.Trim(),
                        StringComparison.OrdinalIgnoreCase));

                if (role is null)
                {
                    var error = string.IsNullOrWhiteSpace(name)
                        ? Error.Validation(
                            "Identity.RoleRequired",
                            "A user must keep at least one role.")
                        : Error.Validation(
                            "Identity.InvalidRole",
                            $"'{name}' is not a valid role. Allowed roles: Tourist, TourGuide, Admin.");

                    return Result.Failure<UserReadDto>(error);
                }

                if (!roles.Contains(role))
                {
                    roles.Add(role);
                }
            }

            if (roles.Count == 0)
            {
                return Result.Failure<UserReadDto>(
                    Error.Validation(
                        "Identity.RoleRequired",
                        "A user must keep at least one role."));
            }
        }

        if (await _users.EmailExistsAsync(request.Email, cancellationToken))
        {
            return Result.Failure<UserReadDto>(
                Error.Conflict(
                    "Identity.EmailTaken",
                    $"An account with e-mail '{request.Email.Trim()}' already exists."));
        }

        UserEntity user;

        try
        {
            user = UserEntity.Create(
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

        foreach (var roleName in roles)
        {
            var role = await _roles.GetByNameAsync(roleName, cancellationToken);

            if (role is null)
            {
                role = Role.Create(roleName);
                _roles.Add(role);
            }

            user.AssignRole(role);
        }

        _users.Add(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return UserReadDto.FromEntity(user);
    }
}