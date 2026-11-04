using MediatR;
using Voyara.Application.Abstractions;
using Voyara.Application.Abstractions.Authorization;
using Voyara.Application.DTOs.Identity;
using Voyara.Domain.Entities.Identity;
using Voyara.Domain.Repositories.Identity;
using Voyara.SharedKernel.Results;

namespace Voyara.Application.Identity.Commands.Users.UpdateUser;

public sealed class UpdateUserCommandHandler
    : IRequestHandler<UpdateUserCommand, Result<UserReadDto>>
{
    private static readonly string[] AllowedRoles =
        [Roles.Tourist, Roles.TourGuide, Roles.Admin];

    private readonly IUserRepository _users;
    private readonly IRoleRepository _roles;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUser _currentUser;

    public UpdateUserCommandHandler(
        IUserRepository users,
        IRoleRepository roles,
        IUnitOfWork unitOfWork,
        ICurrentUser currentUser)
    {
        _users = users;
        _roles = roles;
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }

    public async Task<Result<UserReadDto>> Handle(
        UpdateUserCommand request,
        CancellationToken cancellationToken)
    {
        var user = await _users.GetByIdAsync(
            request.Id,
            null,
            cancellationToken);

        if (user is null)
        {
            return Result.Failure<UserReadDto>(
                Error.NotFound("Identity.NotFound", "User was not found."));
        }

        List<string>? targetRoles = null;

        if (request.Roles is not null)
        {
            if (!TryNormalizeRoles(request.Roles, out var normalized, out var invalidRole))
            {
                var error = invalidRole is null
                    ? Error.Validation(
                        "Identity.RoleRequired",
                        "A user must keep at least one role.")
                    : Error.Validation(
                        "Identity.InvalidRole",
                        $"'{invalidRole}' is not a valid role. Allowed roles: Tourist, TourGuide, Admin.");

                return Result.Failure<UserReadDto>(error);
            }

            if (user.Id == _currentUser.UserId && !normalized.Contains(Roles.Admin))
            {
                return Result.Failure<UserReadDto>(
                    Error.Conflict(
                        "Identity.SelfAdminRemoval",
                        "You cannot remove the Admin role from your own account."));
            }

            targetRoles = normalized;
        }

        Error? failure;

        try
        {
            user.Rename(request.FullName);

            failure = null;
            if (request.IsActive is not null && request.IsActive != user.IsActive)
            {
                if (!request.IsActive.Value && user.Id == _currentUser.UserId)
                {
                    return Result.Failure<UserReadDto>(
                        Error.Conflict(
                            "Identity.SelfDeactivation",
                            "You cannot deactivate your own account."));
                }

                if (request.IsActive.Value)
                {
                    user.Activate();
                }
                else
                {
                    user.Deactivate(DateTimeOffset.UtcNow);
                }
            }
        }
        catch (InvalidOperationException ex)
        {
            failure = Error.Conflict("Identity.InvalidState", ex.Message);
        }
        catch (ArgumentException ex)
        {
            failure = Error.Validation("Identity.InvalidArgument", ex.Message);
        }

        if (failure is not null)
        {
            return Result.Failure<UserReadDto>(failure);
        }

        if (targetRoles is not null)
        {
            foreach (var role in user.Roles
                .Where(role => !targetRoles.Contains(
                    role.Name,
                    StringComparer.OrdinalIgnoreCase))
                .ToList())
            {
                user.RemoveRole(role);
            }

            foreach (var roleName in targetRoles.Where(name => !user.HasRole(name)))
            {
                var role = await _roles.GetByNameAsync(roleName, cancellationToken);

                if (role is null)
                {
                    role = Role.Create(roleName);
                    _roles.Add(role);
                }

                user.AssignRole(role);
            }
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return UserReadDto.FromEntity(user);
    }

    private static bool TryNormalizeRoles(
        IEnumerable<string> names,
        out List<string> roles,
        out string? invalidRole)
    {
        roles = [];
        invalidRole = null;

        foreach (var name in names)
        {
            var canonicalRole = AllowedRoles.FirstOrDefault(role =>
                string.Equals(
                    role,
                    name?.Trim(),
                    StringComparison.OrdinalIgnoreCase));

            if (canonicalRole is null)
            {
                invalidRole = name;
                return false;
            }

            if (!roles.Contains(canonicalRole))
            {
                roles.Add(canonicalRole);
            }
        }

        return roles.Count > 0;
    }
}