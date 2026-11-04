using Microsoft.AspNetCore.Identity;
using Voyara.Application.Abstractions;

namespace Voyara.Api.Security;

/// <summary>
/// <see cref="IPasswordHasher"/> backed by the ASP.NET Core Identity hasher (PBKDF2 with a random salt).
/// </summary>
public sealed class AspNetPasswordHasher : Voyara.Application.Abstractions.IPasswordHasher
{
    private static readonly PasswordHasher<object> Hasher = new();
    private static readonly object Subject = new();

    /// <inheritdoc />
    public string Hash(string password) => Hasher.HashPassword(Subject, password);

    /// <inheritdoc />
    public bool Verify(string passwordHash, string password)
        => Hasher.VerifyHashedPassword(Subject, passwordHash, password) != PasswordVerificationResult.Failed;
}
