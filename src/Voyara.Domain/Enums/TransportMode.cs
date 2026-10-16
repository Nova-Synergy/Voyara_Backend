namespace Voyara.Domain.Enums;

/// <summary>
/// Specifies the mode of transportation used to transit to or explore a journey stop.
/// </summary>
public enum TransportMode
{
    /// <summary>
    /// Foot transit or pedestrian walking between waypoints.
    /// </summary>
    Walking,

    /// <summary>
    /// Private automobile, rental car, or taxi/ride-hailing service.
    /// </summary>
    Driving,

    /// <summary>
    /// Mass transit systems, including metro, buses, trams, and regional trains.
    /// </summary>
    PublicTransport,

    /// <summary>
    /// Commercial or charter aviation between distant cities or regions.
    /// </summary>
    Flight,

    /// <summary>
    /// Bicycle, electric scooter, or personal micro-mobility equipment.
    /// </summary>
    Cycling,

    /// <summary>
    /// Watercraft transit, such as ferries, cruises, or boat charters.
    /// </summary>
    Boat
}










