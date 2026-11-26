namespace Voyara.Application.DTOs;

/// <summary>Token pair returned after login or a refresh-token rotation.</summary>
public sealed record AuthTokensResponse(
    string AccessToken,
    DateTimeOffset AccessTokenExpiresAt,
    string RefreshToken,
    DateTimeOffset RefreshTokenExpiresAt,
    string TokenType = "Bearer");
