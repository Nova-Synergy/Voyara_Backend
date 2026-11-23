using MediatR;
using Voyara.Application.Abstractions;
using Voyara.Application.Abstractions.Identity;
using Voyara.Application.DTOs;
using Voyara.Domain.Repositories.Identity;
using Voyara.SharedKernel.Results;

namespace Voyara.Application.Identity.Commands.RefreshAccessToken;

/// <summary>
/// Rotates a refresh token: the presented token is revoked and a new pair is issued. Presenting a token
/// that was already revoked is treated as theft and ends every session of the user.
/// </summary>
public sealed class RefreshAccessTokenCommandHandler
    : IRequestHandler<RefreshAccessTokenCommand, Result<AuthTokensResponse>>
{
    private static readonly Error InvalidToken =
        Error.Unauthenticated("Auth.InvalidRefreshToken", "The refresh token is invalid or has expired.");

    private readonly IUserRepository _userRepository;
    private readonly ITokenService _tokenService;
    private readonly IDateTime _dateTime;
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>Creates the handler.</summary>
    public RefreshAccessTokenCommandHandler(
        IUserRepository userRepository,
        ITokenService tokenService,
        IDateTime dateTime,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _tokenService = tokenService;
        _dateTime = dateTime;
        _unitOfWork = unitOfWork;
    }

    /// <inheritdoc />
    public async Task<Result<AuthTokensResponse>> Handle(
        RefreshAccessTokenCommand request,
        CancellationToken cancellationToken)
    {
        var presentedHash = _tokenService.HashRefreshToken(request.RefreshToken);
        var user = await _userRepository.GetByRefreshTokenHashAsync(presentedHash, cancellationToken);
        if (user is null)
        {
            return Result.Failure<AuthTokensResponse>(InvalidToken);
        }

        var now = _dateTime.UtcNow;

        if (user.FindActiveRefreshToken(presentedHash, now) is null)
        {
            // Revoked tokens are never presented by a well-behaved client: assume theft.
            var reused = user.RefreshTokens.Any(t => t.TokenHash == presentedHash && t.RevokedAt is not null);
            if (reused)
            {
                user.RevokeAllRefreshTokens(now);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }

            return Result.Failure<AuthTokensResponse>(InvalidToken);
        }

        if (!user.IsActive)
        {
            return Result.Failure<AuthTokensResponse>(
                Error.Unauthenticated("Auth.AccountDeactivated", "This account is deactivated."));
        }

        user.RevokeRefreshToken(presentedHash, now);

        var newRefreshToken = _tokenService.CreateRefreshToken();
        user.IssueRefreshToken(newRefreshToken.Hash, newRefreshToken.ExpiresAt, now);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var accessToken = _tokenService.CreateAccessToken(user);

        return new AuthTokensResponse(
            accessToken.Value,
            accessToken.ExpiresAt,
            newRefreshToken.Value,
            newRefreshToken.ExpiresAt);
    }
}
