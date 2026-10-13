using Microsoft.AspNetCore.Authorization;
using Voyara.Application.Abstractions.Authorization;

namespace Voyara.Api.Authorization;

/// <summary>Named authorization policies used on controllers.</summary>
public static class AuthorizationPolicies
{
    /// <summary>Restricts access to administrators.</summary>
    public const string AdminOnly = "AdminOnly";

    /// <summary>Restricts access to tourists.</summary>
    public const string TouristOnly = "TouristOnly";

    /// <summary>Restricts access to tour guides.</summary>
    public const string GuideOnly = "GuideOnly";

    /// <summary>Registers the role-based policies.</summary>
    public static void Register(AuthorizationOptions options)
    {
        options.AddPolicy(AdminOnly, policy => policy.RequireRole(Roles.Admin));
        options.AddPolicy(TouristOnly, policy => policy.RequireRole(Roles.Tourist));
        options.AddPolicy(GuideOnly, policy => policy.RequireRole(Roles.TourGuide));
    }
}
