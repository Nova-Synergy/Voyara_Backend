namespace Voyara.Application.Abstractions;

/// <summary>Password hashing contract; implemented by the host so the Application layer stays framework-free.</summary>
public interface IPasswordHasher
{
    /// <summary>Hashes a raw password.</summary>
    string Hash(string password);

    /// <summary>Checks a raw password against a stored hash.</summary>
    bool Verify(string passwordHash, string password);
}
