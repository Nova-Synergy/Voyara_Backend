using MediatR;
using Voyara.Application.Abstractions;
using Voyara.Application.Abstractions.Authorization;
using Voyara.Domain.Repositories.Identity;
using Voyara.SharedKernel.Results;

namespace Voyara.Application.Identity.Commands.Admins.DeleteAdmin;

public sealed class DeleteAdminCommandHandler
    : IRequestHandler<DeleteAdminCommand, Result>
{
    private readonly IUserRepository _users;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUser _currentUser;

    public DeleteAdminCommandHandler(
        IUserRepository users,
        IUnitOfWork unitOfWork,
        ICurrentUser currentUser)
    {
        _users = users;
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }

    public async Task<Result> Handle(
        DeleteAdminCommand request,
        CancellationToken cancellationToken)
    {
        if (request.Id == _currentUser.UserId)
        {
            return Result.Failure(
                Error.Conflict(
                    "Identity.SelfDeletion",
                    "You cannot delete your own account."));
        }

        var admin = await _users.GetByIdAsync(
            request.Id,
            Roles.Admin,
            cancellationToken);

        if (admin is null)
        {
            return Result.Failure(
                Error.NotFound(
                    "Identity.NotFound",
                    "Administrator was not found."));
        }

        var now = DateTimeOffset.UtcNow;

        if (admin.IsActive)
        {
            admin.Deactivate(now);
        }
        else
        {
            admin.RevokeAllRefreshTokens(now);
        }

        admin.IsDeleted = true;

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}