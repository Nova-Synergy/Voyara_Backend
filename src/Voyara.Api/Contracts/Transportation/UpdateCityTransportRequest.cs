namespace Voyara.Api.Contracts.Transportation;

/// <summary>Request body for updating the details of a city transport.</summary>
public sealed record UpdateCityTransportRequest(
    string Description,
    decimal ApproxCost,
    string Currency,
    string OperatingHours,
    string? SafetyInfo);
