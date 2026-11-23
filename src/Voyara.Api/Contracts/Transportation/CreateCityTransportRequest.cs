namespace Voyara.Api.Contracts.Transportation;

/// <summary>Request body to create the transport information of one transport type in one city.</summary>
/// <param name="CityId">Identifier of the city.</param>
/// <param name="TransportTypeId">Identifier of the transport type.</param>
/// <param name="Description">General description (required, up to 2000 characters).</param>
/// <param name="ApproxCost">Approximate cost of a typical ride; not negative.</param>
/// <param name="Currency">Three-letter ISO 4217 currency code.</param>
/// <param name="OperatingHours">Free-text operating hours (required, up to 200 characters).</param>
/// <param name="SafetyInfo">Optional safety tips (up to 2000 characters).</param>
public sealed record CreateCityTransportRequest(
    Guid CityId,
    int TransportTypeId,
    string Description,
    decimal ApproxCost,
    string Currency,
    string OperatingHours,
    string? SafetyInfo);
