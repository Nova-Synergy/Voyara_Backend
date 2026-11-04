using MediatR;
using Voyara.Application.Abstractions;
using Voyara.Application.Abstractions.Authorization;
using Voyara.Application.DTOs.Identity;
using Voyara.Domain.Repositories.Identity;
using Voyara.SharedKernel.Results;

namespace Voyara.Application.Identity.Commands.Admins.UpdateAdmin;

public sealed class UpdateAdminCommandHandler
    : IRequestHandler<UpdateAdminCommand, Result<UserReadDto>>
{
    private readonly IUserRepository _users;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUser _currentUser;

    public UpdateAdminCommandHandler(
        IUserRepository users,
        IUnitOfWork unitOfWork,
        ICurrentUser currentUser)
    {
        _users = users;
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }

    public async Task<Result<UserReadDto>> Handle(
        UpdateAdminCommand request,
        CancellationToken cancellationToken)
    {
        var admin = await _users.GetByIdAsync(
            request.Id,
            Roles.Admin,
            cancellationToken);

        if (admin is null)
        {
            return Result.Failure<UserReadDto>(
                Error.NotFound(
                    "Identity.NotFound",
                    "Administrator was not found."));
        }

        try
        {
            admin.Rename(request.FullName);

            if (request.IsActive is not null && request.IsActive != admin.IsActive)
            {
                if (!request.IsActive.Value && admin.Id == _currentUser.UserId)
                {
                    return Result.Failure<UserReadDto>(
                        Error.Conflict(
                            "Identity.SelfDeactivation",
                            "You cannot deactivate your own account."));
                }

                if (request.IsActive.Value)
                {
                    admin.Activate();
                }
                else
                {
                    admin.Deactivate(DateTimeOffset.UtcNow);
                }
            }
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

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return UserReadDto.FromEntity(admin);
    }
}