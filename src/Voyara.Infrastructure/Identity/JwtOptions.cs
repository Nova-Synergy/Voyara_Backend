namespace Voyara.Infrastructure.Identity;

/// <summary>JWT settings bound from configuration (section "Jwt").</summary>
public sealed class JwtOptions
{
    /// <summary>Configuration section name.</summary>
    public const string SectionName = "Jwt";

    /// <summary>Minimum signing key length in bytes required by HMAC-SHA256.</summary>
    public const int MinSigningKeyLength = 32;

    /// <summary>Expected token issuer.</summary>
    public string Issuer { get; set; } = string.Empty;

    /// <summary>Expected token audience.</summary>
    public string Audience { get; set; } = string.Empty;

    /// <summary>Token signing key; supplied per environment, never hard-coded.</summary>
    public string SigningKey { get; set; } = string.Empty;

    /// <summary>Access token lifetime in minutes.</summary>
    public int ExpirationMinutes { get; set; } = 60;

    /// <summary>Refresh token lifetime in days.</summary>
    public int RefreshTokenExpirationDays { get; set; } = 14;
}
