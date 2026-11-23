using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Voyara.Application.Abstractions;
using Voyara.Application.Abstractions.Identity;
using Voyara.Domain.Entities.Identity;

namespace Voyara.Infrastructure.Identity;

/// <summary>
/// Issues HMAC-SHA256 signed JWT access tokens and opaque random refresh tokens. Refresh tokens are
/// persisted only as SHA-256 hashes; they carry 512 bits of entropy so no salt or slow hash is needed.
/// </summary>
public sealed class JwtTokenService : ITokenService
{
    /// <summary>Claim type used for roles; the API validates with the same name.</summary>
    public const string RoleClaimType = "role";

    private readonly JwtOptions _options;
    private readonly IDateTime _dateTime;

    /// <summary>Creates the service.</summary>
    public JwtTokenService(IOptions<JwtOptions> options, IDateTime dateTime)
    {
        _options = options.Value;
        _dateTime = dateTime;
    }

    /// <inheritdoc />
    public AccessToken CreateAccessToken(User user)
    {
        ArgumentNullException.ThrowIfNull(user);

        var keyBytes = Encoding.UTF8.GetBytes(_options.SigningKey ?? string.Empty);
        if (keyBytes.Length < JwtOptions.MinSigningKeyLength)
        {
            throw new InvalidOperationException(
                $"Jwt:SigningKey must be at least {JwtOptions.MinSigningKeyLength} bytes long.");
        }

        var now = _dateTime.UtcNow;
        var expiresAt = now.AddMinutes(_options.ExpirationMinutes);

        var claims = new List<System.Security.Claims.Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new(System.Security.Claims.ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(System.Security.Claims.ClaimTypes.Email, user.Email),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N"))
        };
        claims.AddRange(user.Roles.Select(role => new System.Security.Claims.Claim(RoleClaimType, role.Name)));

        var token = new JwtSecurityToken(
            issuer: string.IsNullOrWhiteSpace(_options.Issuer) ? null : _options.Issuer,
            audience: string.IsNullOrWhiteSpace(_options.Audience) ? null : _options.Audience,
            claims: claims,
            notBefore: now.UtcDateTime,
            expires: expiresAt.UtcDateTime,
            signingCredentials: new SigningCredentials(
                new SymmetricSecurityKey(keyBytes), SecurityAlgorithms.HmacSha256));

        return new AccessToken(new JwtSecurityTokenHandler().WriteToken(token), expiresAt);
    }

    /// <inheritdoc />
    public GeneratedRefreshToken CreateRefreshToken()
    {
        var raw = ToBase64Url(RandomNumberGenerator.GetBytes(64));
        var expiresAt = _dateTime.UtcNow.AddDays(_options.RefreshTokenExpirationDays);

        return new GeneratedRefreshToken(raw, HashRefreshToken(raw), expiresAt);
    }

    /// <inheritdoc />
    public string HashRefreshToken(string rawToken)
    {
        ArgumentNullException.ThrowIfNull(rawToken);
        return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(rawToken)));
    }

    private static string ToBase64Url(byte[] bytes)
        => Base64UrlEncoder.Encode(bytes);
}
