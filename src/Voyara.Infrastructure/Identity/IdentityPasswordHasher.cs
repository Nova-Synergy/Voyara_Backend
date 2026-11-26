using Microsoft.AspNetCore.Identity;
using Voyara.Application.Abstractions.Identity;
using Voyara.Domain.Entities.Identity;

namespace Voyara.Infrastructure.Identity;

/// <summary>
/// Password hashing delegated to ASP.NET Core Identity's <see cref="PasswordHasher{TUser}"/> (the same
/// hasher <c>UserManager</c> uses: PBKDF2-HMAC-SHA512, versioned format). It is used on its own so the
/// Domain <see cref="User"/> stays free of Identity base classes.
/// </summary>
public sealed class IdentityPasswordHasher : IPasswordHasher
{
    private static readonly PasswordHasher<User> Hasher = new();

    // Verified against when the stored hash is missing, so unknown e-mails cost the same time as real ones.
    private static readonly string DummyHash = Hasher.HashPassword(null!, "Dummy#Password1");

    /// <inheritdoc />
    public string Hash(string password)
    {
        ArgumentException.ThrowIfNullOrEmpty(password);

        // The default hasher ignores the user argument.
        return Hasher.HashPassword(null!, password);
    }

    /// <inheritdoc />
    public bool Verify(string password, string passwordHash)
    {
        if (string.IsNullOrEmpty(passwordHash))
        {
            Hasher.VerifyHashedPassword(null!, DummyHash, password ?? string.Empty);
            return false;
        }

        var result = Hasher.VerifyHashedPassword(null!, passwordHash, password ?? string.Empty);
        return result != PasswordVerificationResult.Failed;
    }
}
