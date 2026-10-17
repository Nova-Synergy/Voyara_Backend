using Voyara.Domain.ValueObjects;

namespace Voyara.Domain.Entities.Transportation;

/// <summary>
/// Data needed to create a <see cref="Station"/>. Stations are created only by their aggregate root,
/// so callers describe them with this input instead of constructing entities themselves.
/// </summary>
/// <param name="Name">Display name of the station.</param>
/// <param name="Location">Geographic location of the station.</param>
public sealed record StationInput(string Name, GeoPoint Location);
