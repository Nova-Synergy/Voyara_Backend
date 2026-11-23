using Voyara.Application.Abstractions.Messaging;

namespace Voyara.Application.Identity.Commands.Logout;

/// <summary>
/// Ends one session by revoking its refresh token. Holding the token is the proof of ownership, so no
/// access token is required (it may already have expired). Unknown tokens succeed silently.
/// </summary>
public sealed record LogoutCommand(string RefreshToken) : ICommand;
