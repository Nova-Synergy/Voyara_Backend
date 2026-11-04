using MediatR;
using Voyara.Application.Abstractions;
using Voyara.Application.Abstractions.Authorization;
using Voyara.Domain.Repositories.Identity;
using Voyara.SharedKernel.Results;

namespace Voyara.Application.Identity.Commands.Tourists.DeleteTourist;

public sealed class DeleteTouristCommandHandler
    : IRequestHandler<DeleteTouristCommand, Result>
{
    private readonly IUserRepository _users;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUser _currentUser;

    public DeleteTouristCommandHandler(
        IUserRepository users,
        IUnitOfWork unitOfWork,
        ICurrentUser currentUser)
    {
        _users = users;
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }

    public async Task<Result> Handle(
        DeleteTouristCommand request,
        CancellationToken cancellationToken)
    {
        if (!_currentUser.IsInRole(Roles.Admin)
            && _currentUser.UserId != request.UserId)
        {
            return Result.Failure(
                Error.Unauthorized(
                    "Identity.Forbidden",
                    "You are not allowed to access this account."));
        }

        var user = await _users.GetByIdAsync(
            request.UserId,
            Roles.Tourist,
            cancellationToken);

        if (user is null)
        {
            return Result.Failure(
                Error.NotFound(
                    "Identity.NotFound",
                    "Tourist was not found."));
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