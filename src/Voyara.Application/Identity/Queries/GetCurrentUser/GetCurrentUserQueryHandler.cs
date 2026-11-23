using MediatR;
using Voyara.Application.Abstractions;
using Voyara.Application.DTOs;
using Voyara.Domain.Repositories.Identity;
using Voyara.SharedKernel.Results;

namespace Voyara.Application.Identity.Queries.GetCurrentUser;

/// <summary>Loads the account identified by the caller's token.</summary>
public sealed class GetCurrentUserQueryHandler : IRequestHandler<GetCurrentUserQuery, Result<CurrentUserResponse>>
{
    private readonly ICurrentUser _currentUser;
    private readonly IUserRepository _userRepository;

    /// <summary>Creates the handler.</summary>
    public GetCurrentUserQueryHandler(ICurrentUser currentUser, IUserRepository userRepository)
    {
        _currentUser = currentUser;
        _userRepository = userRepository;
    }

    /// <inheritdoc />
    public async Task<Result<CurrentUserResponse>> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken)
    {
        if (_currentUser.UserId is not { } userId)
        {
            return Result.Failure<CurrentUserResponse>(
                Error.Unauthenticated("Authorization.Anonymous", "Authentication is required."));
        }

        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
        if (user is null)
        {
            return Result.Failure<CurrentUserResponse>(
                Error.NotFound("User.NotFound", "The authenticated user no longer exists."));
        }

        if (!user.IsActive)
        {
            return Result.Failure<CurrentUserResponse>(
                Error.Unauthenticated("Auth.AccountDeactivated", "This account is deactivated."));
        }

        return new CurrentUserResponse(
            user.Id,
            user.FullName,
            user.Email,
            user.EmailConfirmed,
            user.Roles.Select(r => r.Name).OrderBy(n => n).ToList(),
            user.CreatedAt);
    }
}
