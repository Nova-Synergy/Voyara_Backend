using Microsoft.AspNetCore.Identity;

namespace Voyara.Api.Security;

/// <summary>
/// Password hashing for account creation, using the ASP.NET Core Identity hasher (PBKDF2 with a random salt).
/// A future login endpoint verifies with <see cref="Verify"/>.
/// </summary>
public static class PasswordHashing
{
    private static readonly PasswordHasher<object> Hasher = new();
    private static readonly object Subject = new();

    /// <summary>Hashes a raw password.</summary>
    public static string Hash(string password) => Hasher.HashPassword(Subject, password);

    /// <summary>Checks a raw password against a stored hash.</summary>
    public static bool Verify(string passwordHash, string password)
        => Hasher.VerifyHashedPassword(Subject, passwordHash, password) != PasswordVerificationResult.Failed;
}
