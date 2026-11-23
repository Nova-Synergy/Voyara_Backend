namespace Voyara.Api.Contracts.Transportation;

/// <summary>Request body to change the details of a city transport. City and transport type cannot change.</summary>
/// <param name="Description">General description (required, up to 2000 characters).</param>
/// <param name="ApproxCost">Approximate cost of a typical ride; not negative.</param>
/// <param name="Currency">Three-letter ISO 4217 currency code.</param>
/// <param name="OperatingHours">Free-text operating hours (required, up to 200 characters).</param>
/// <param name="SafetyInfo">Optional safety tips (up to 2000 characters).</param>
public sealed record UpdateCityTransportRequest(
    string Description,
    decimal ApproxCost,
    string Currency,
    string OperatingHours,
    string? SafetyInfo);
