namespace Voyara.Application.Abstractions.Authorization;

/// <summary>Application role names shared by the authorization behavior and API policies.</summary>
public static class Roles
{
    /// <summary>Tourist role.</summary>
    public const string Tourist = "Tourist";

    /// <summary>Tour guide role.</summary>
    public const string TourGuide = "TourGuide";

    /// <summary>Administrator role.</summary>
    public const string Admin = "Admin";
}
