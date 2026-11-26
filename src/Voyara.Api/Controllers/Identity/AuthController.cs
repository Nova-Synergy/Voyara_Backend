using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Voyara.Api.Contracts.Identity;
using Voyara.Application.Identity.Commands.Login;
using Voyara.Application.Identity.Commands.Logout;
using Voyara.Application.Identity.Commands.RefreshAccessToken;
using Voyara.Application.Identity.Commands.RegisterUser;

namespace Voyara.Api.Controllers.Identity;

/// <summary>Anonymous authentication endpoints: registration, login, token rotation and logout.</summary>
[AllowAnonymous]
[Route("api/v1/auth")]
public sealed class AuthController : BaseApiController
{
    /// <summary>Registers a new tourist account.</summary>
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request, CancellationToken cancellationToken)
    {
        var result = await Sender.Send(
            new RegisterUserCommand(request.FullName, request.Email, request.Password),
            cancellationToken);

        return result.IsSuccess
            ? StatusCode(StatusCodes.Status201Created, result.Value)
            : HandleResult(result);
    }

    /// <summary>Logs in with e-mail and password and returns an access/refresh token pair.</summary>
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request, CancellationToken cancellationToken)
        => HandleResult(await Sender.Send(new LoginCommand(request.Email, request.Password), cancellationToken));

    /// <summary>Exchanges a refresh token for a new token pair (the old refresh token is revoked).</summary>
    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh(RefreshTokenRequest request, CancellationToken cancellationToken)
        => HandleResult(await Sender.Send(new RefreshAccessTokenCommand(request.RefreshToken), cancellationToken));

    /// <summary>Ends the session by revoking its refresh token.</summary>
    [HttpPost("logout")]
    public async Task<IActionResult> Logout(LogoutRequest request, CancellationToken cancellationToken)
    {
        var result = await Sender.Send(new LogoutCommand(request.RefreshToken), cancellationToken);

        return result.IsSuccess ? NoContent() : HandleResult(result);
    }
}
