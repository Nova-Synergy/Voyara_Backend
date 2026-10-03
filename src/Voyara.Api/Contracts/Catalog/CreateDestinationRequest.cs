namespace Voyara.Api.Contracts.Catalog;

/// <summary>Request body for creating a destination.</summary>
public sealed record CreateDestinationRequest(string Name, string? Description, double Latitude, double Longitude);
