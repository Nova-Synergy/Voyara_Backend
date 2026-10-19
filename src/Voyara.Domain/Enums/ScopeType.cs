namespace Voyara.Domain.Enums;

/// <summary>
/// Defines the geographic level to which a local rule applies.
/// </summary>
public enum ScopeType
{
    /// <summary>The rule applies to a whole country.</summary>
    Country,

    /// <summary>The rule applies to a city.</summary>
    City,

    /// <summary>The rule applies to a single destination.</summary>
    Destination
}
