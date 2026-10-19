namespace Voyara.Domain.Enums;

/// <summary>
/// Defines the units used to determine the pricing of a guide service.
/// </summary>
public enum PricingUnit
{
    /// <summary>
    /// The price is calculated per hour.
    /// </summary>
    PerHour,

    /// <summary>
    /// The price is calculated for a half-day service.
    /// </summary>
    HalfDay,

    /// <summary>
    /// The price is calculated for a full-day service.
    /// </summary>
    FullDay
}
