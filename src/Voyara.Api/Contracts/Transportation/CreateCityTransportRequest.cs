namespace Voyara.Api.Contracts.Transportation;

/// <summary>Request body for creating a city transport.</summary>
public sealed record CreateCityTransportRequest(
    int CityId,
    int TransportTypeId,
    string Description,
    decimal ApproxCost,
    string Currency,
    string OperatingHours,
    string? SafetyInfo);
