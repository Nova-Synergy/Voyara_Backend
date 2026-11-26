namespace Voyara.Application.Abstractions.Identity;

/// <summary>One-way password hashing; the raw password is never stored.</summary>
public interface IPasswordHasher
{
    /// <summary>Returns a salted hash of the password.</summary>
    string Hash(string password);

    /// <summary>Checks a password against a hash produced by <see cref="Hash"/> in constant time.</summary>
    bool Verify(string password, string passwordHash);
}
