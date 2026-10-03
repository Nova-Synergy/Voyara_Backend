namespace Voyara.Application.Abstractions;

/// <summary>Access to the current caller's identity, populated by the host (HTTP or worker) context.</summary>
public interface ICurrentUser
{
    /// <summary>Identifier of the authenticated user; null when anonymous.</summary>
    Guid? UserId { get; }

    /// <summary>Email claim of the authenticated user; null when absent.</summary>
    string? Email { get; }

    /// <summary>Indicates whether a user is authenticated.</summary>
    bool IsAuthenticated { get; }

    /// <summary>Checks whether the user holds one of the application roles (Tourist, TourGuide, Admin).</summary>
    bool IsInRole(string role);
}
