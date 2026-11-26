using MediatR;
using Voyara.Application.Abstractions;
using Voyara.Application.Abstractions.Identity;
using Voyara.Application.DTOs;
using Voyara.Domain.Repositories.Identity;
using Voyara.SharedKernel.Results;

namespace Voyara.Application.Identity.Commands.Login;

/// <summary>Verifies the credentials and issues tokens; never reveals which part of the credentials was wrong.</summary>
public sealed class LoginCommandHandler : IRequestHandler<LoginCommand, Result<AuthTokensResponse>>
{
    private static readonly Error InvalidCredentials =
        Error.Unauthenticated("Auth.InvalidCredentials", "Invalid e-mail or password.");

    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;
    private readonly IDateTime _dateTime;
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>Creates the handler.</summary>
    public LoginCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        ITokenService tokenService,
        IDateTime dateTime,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
        _dateTime = dateTime;
        _unitOfWork = unitOfWork;
    }

    /// <inheritdoc />
    public async Task<Result<AuthTokensResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);

        if (user is null)
        {
            // Spend the same time as a real verification so response timing does not reveal unknown e-mails.
            _passwordHasher.Verify(request.Password, string.Empty);
            return Result.Failure<AuthTokensResponse>(InvalidCredentials);
        }

        if (!_passwordHasher.Verify(request.Password, user.PasswordHash))
        {
            return Result.Failure<AuthTokensResponse>(InvalidCredentials);
        }

        if (!user.IsActive)
        {
            return Result.Failure<AuthTokensResponse>(
                Error.Unauthenticated("Auth.AccountDeactivated", "This account is deactivated."));
        }

        var now = _dateTime.UtcNow;
        var refreshToken = _tokenService.CreateRefreshToken();
        user.IssueRefreshToken(refreshToken.Hash, refreshToken.ExpiresAt, now);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var accessToken = _tokenService.CreateAccessToken(user);

        return new AuthTokensResponse(
            accessToken.Value,
            accessToken.ExpiresAt,
            refreshToken.Value,
            refreshToken.ExpiresAt);
    }
}
