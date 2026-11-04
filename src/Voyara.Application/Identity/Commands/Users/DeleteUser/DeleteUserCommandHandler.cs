using MediatR;
using Voyara.Application.Abstractions;
using Voyara.Domain.Repositories.Identity;
using Voyara.SharedKernel.Results;

namespace Voyara.Application.Identity.Commands.Users.DeleteUser;

public sealed class DeleteUserCommandHandler
    : IRequestHandler<DeleteUserCommand, Result>
{
    private readonly IUserRepository _users;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUser _currentUser;

    public DeleteUserCommandHandler(
        IUserRepository users,
        IUnitOfWork unitOfWork,
        ICurrentUser currentUser)
    {
        _users = users;
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }

    public async Task<Result> Handle(
        DeleteUserCommand request,
        CancellationToken cancellationToken)
    {
        if (request.Id == _currentUser.UserId)
        {
            return Result.Failure(
                Error.Conflict(
                    "Identity.SelfDeletion",
                    "You cannot delete your own account."));
        }

        var user = await _users.GetByIdAsync(
            request.Id,
            null,
            cancellationToken);

        if (user is null)
        {
            return Result.Failure(
                Error.NotFound(
                    "Identity.NotFound",
                    "User was not found."));
        }

        var now = DateTimeOffset.UtcNow;

        if (user.IsActive)
        {
            user.Deactivate(now);
        }
        else
        {
            user.RevokeAllRefreshTokens(now);
        }

        user.IsDeleted = true;

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}