using Voyara.SharedKernel.Results;

namespace Voyara.Domain.Entities.Transportation;

/// <summary>Domain errors raised by the <see cref="CityTransport"/> aggregate and its parts.</summary>
public static class CityTransportErrors
{
    /// <summary>The city identifier is not a positive number.</summary>
    public static readonly Error InvalidCityId = Error.Validation(
        "CityTransport.InvalidCityId", "City id must be a positive number.");

    /// <summary>The transport type identifier is not a positive number.</summary>
    public static readonly Error InvalidTransportTypeId = Error.Validation(
        "CityTransport.InvalidTransportTypeId", "Transport type id must be a positive number.");

    /// <summary>The description is empty or too long.</summary>
    public static readonly Error InvalidDescription = Error.Validation(
        "CityTransport.InvalidDescription", "Description is required and must not exceed 2000 characters.");

    /// <summary>The approximate cost is negative.</summary>
    public static readonly Error InvalidApproxCost = Error.Validation(
        "CityTransport.InvalidApproxCost", "Approximate cost cannot be negative.");

    /// <summary>The currency is not a three-letter code.</summary>
    public static readonly Error InvalidCurrency = Error.Validation(
        "CityTransport.InvalidCurrency", "Currency must be a three-letter ISO 4217 code.");

    /// <summary>The operating hours are empty or too long.</summary>
    public static readonly Error InvalidOperatingHours = Error.Validation(
        "CityTransport.InvalidOperatingHours", "Operating hours are required and must not exceed 200 characters.");

    /// <summary>The safety information is too long.</summary>
    public static readonly Error InvalidSafetyInfo = Error.Validation(
        "CityTransport.InvalidSafetyInfo", "Safety information must not exceed 2000 characters.");

    /// <summary>The route name is empty or too long.</summary>
    public static readonly Error InvalidRouteName = Error.Validation(
        "CityTransport.InvalidRouteName", "Route name is required and must not exceed 200 characters.");

    /// <summary>The route description is too long.</summary>
    public static readonly Error InvalidRouteDescription = Error.Validation(
        "CityTransport.InvalidRouteDescription", "Route description must not exceed 2000 characters.");

    /// <summary>The route fare is negative.</summary>
    public static readonly Error InvalidFare = Error.Validation(
        "CityTransport.InvalidFare", "Fare cannot be negative.");

    /// <summary>Another route with the same name already exists.</summary>
    public static readonly Error DuplicateRouteName = Error.Validation(
        "CityTransport.DuplicateRouteName", "A route with the same name already exists.");

    /// <summary>The route does not exist in this city transport.</summary>
    public static readonly Error RouteNotFound = Error.Validation(
        "CityTransport.RouteNotFound", "The route was not found.");

    /// <summary>A route needs at least two stations.</summary>
    public static readonly Error TooFewStations = Error.Validation(
        "CityTransport.TooFewStations", "A route must have at least two stations.");

    /// <summary>The station name is empty or too long.</summary>
    public static readonly Error InvalidStationName = Error.Validation(
        "CityTransport.InvalidStationName", "Station name is required and must not exceed 200 characters.");

    /// <summary>The station location is missing.</summary>
    public static readonly Error InvalidStationLocation = Error.Validation(
        "CityTransport.InvalidStationLocation", "Station location is required.");

    /// <summary>The station does not exist in the route.</summary>
    public static readonly Error StationNotFound = Error.Validation(
        "CityTransport.StationNotFound", "The station was not found.");

    /// <summary>The destination identifier is empty.</summary>
    public static readonly Error InvalidDestinationId = Error.Validation(
        "CityTransport.InvalidDestinationId", "Destination id is required.");

    /// <summary>The destination is already linked.</summary>
    public static readonly Error DuplicateDestination = Error.Validation(
        "CityTransport.DuplicateDestination", "The destination is already linked to this transport.");

    /// <summary>The destination is not linked.</summary>
    public static readonly Error DestinationNotFound = Error.Validation(
        "CityTransport.DestinationNotFound", "The destination is not linked to this transport.");
}
