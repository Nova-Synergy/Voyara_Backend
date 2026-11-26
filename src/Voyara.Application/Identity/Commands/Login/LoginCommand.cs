using Voyara.Application.Abstractions.Messaging;
using Voyara.Application.DTOs;

namespace Voyara.Application.Identity.Commands.Login;

/// <summary>Authenticates with e-mail and password and issues an access/refresh token pair.</summary>
public sealed record LoginCommand(string Email, string Password) : ICommand<AuthTokensResponse>;
