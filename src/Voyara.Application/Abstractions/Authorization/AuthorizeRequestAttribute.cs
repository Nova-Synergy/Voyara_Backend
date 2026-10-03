namespace Voyara.Application.Abstractions.Authorization;

/// <summary>
/// Declares role requirements on a request; enforced by the Application pipeline's
/// authorization behavior without any ASP.NET Core dependency.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public sealed class AuthorizeRequestAttribute : Attribute
{
    /// <summary>Requires an authenticated user of any role.</summary>
    public AuthorizeRequestAttribute()
    {
    }

    /// <summary>Requires an authenticated user holding at least one of the comma-separated roles.</summary>
    public AuthorizeRequestAttribute(string roles)
    {
        Roles = roles;
    }

    /// <summary>Comma-separated allowed roles; null means any authenticated user.</summary>
    public string? Roles { get; }
}
