namespace Voyara.Api.Authorization;

/// <summary>JWT bearer settings bound from configuration (section "Jwt").</summary>
public sealed class JwtOptions
{
    /// <summary>Configuration section name.</summary>
    public const string SectionName = "Jwt";

    /// <summary>Expected token issuer.</summary>
    public string Issuer { get; set; } = string.Empty;

    /// <summary>Expected token audience.</summary>
    public string Audience { get; set; } = string.Empty;

    /// <summary>Token signing key; supplied per environment, never hard-coded.</summary>
    public string SigningKey { get; set; } = string.Empty;

    /// <summary>Token lifetime in minutes.</summary>
    public int ExpirationMinutes { get; set; } = 60;
}
