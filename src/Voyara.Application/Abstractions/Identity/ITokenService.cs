using Voyara.Domain.Entities.Identity;

namespace Voyara.Application.Abstractions.Identity;

/// <summary>A signed access token and its expiry.</summary>
public sealed record AccessToken(string Value, DateTimeOffset ExpiresAt);

/// <summary>
/// A freshly generated refresh token: the raw value goes to the client, only the hash is persisted.
/// </summary>
public sealed record GeneratedRefreshToken(string Value, string Hash, DateTimeOffset ExpiresAt);

/// <summary>Creates access and refresh tokens.</summary>
public interface ITokenService
{
    /// <summary>Creates a short-lived signed access token carrying the user's id, e-mail and roles.</summary>
    AccessToken CreateAccessToken(User user);

    /// <summary>Creates a random refresh token together with its hash and expiry.</summary>
    GeneratedRefreshToken CreateRefreshToken();

    /// <summary>Hashes a raw refresh token the way <see cref="CreateRefreshToken"/> does, for lookups.</summary>
    string HashRefreshToken(string rawToken);
}
