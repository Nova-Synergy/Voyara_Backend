namespace Voyara.Api.Contracts.Catalog;

/// <summary>Request body for creating a destination.</summary>
/// <param name="Name">The destination name in the default language.</param>
/// <param name="Description">Optional destination description.</param>
/// <param name="Latitude">Latitude in degrees.</param>
/// <param name="Longitude">Longitude in degrees.</param>
/// <param name="CityId">GUID identifier of an existing city.</param>
/// <param name="CategoryId">Identifier of an existing category.</param>
public sealed record CreateDestinationRequest(
    string Name,
    string? Description,
    double Latitude,
    double Longitude,
    Guid CityId,
    Guid CategoryId);
