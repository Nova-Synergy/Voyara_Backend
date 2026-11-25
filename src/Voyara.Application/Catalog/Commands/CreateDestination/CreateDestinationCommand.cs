using Voyara.Application.Abstractions.Messaging;

namespace Voyara.Application.Catalog.Commands.CreateDestination;

/// <summary>Creates a destination in Draft status; returns the destination identifier.</summary>
/// <param name="Name">The destination name in the default language.</param>
/// <param name="Description">Optional destination description.</param>
/// <param name="Latitude">Latitude in degrees.</param>
/// <param name="Longitude">Longitude in degrees.</param>
/// <param name="CityId">Identifier of an existing city.</param>
/// <param name="CategoryId">Identifier of an existing category.</param>
public sealed record CreateDestinationCommand(
    string Name,
    string? Description,
    double Latitude,
    double Longitude,
    int CityId,
    Guid CategoryId) : ICommand<Guid>;
