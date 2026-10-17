using Voyara.SharedKernel.Entities;
using Voyara.SharedKernel.Results;

namespace Voyara.Domain.Entities.Transportation;

/// <summary>
/// Aggregate root that describes how a <see cref="TransportType"/> works inside a city: cost, operating hours,
/// safety information, its routes (with their stations) and the destinations it reaches.
/// </summary>
/// <remarks>
/// <para>
/// The aggregate protects these invariants: a route has at least two stations, route names are unique inside
/// the aggregate, and a destination is linked only once.
/// </para>
/// <para>
/// Uniqueness of (<see cref="CityId"/>, <see cref="TransportTypeId"/>) spans several aggregates, so it must be
/// checked by the application layer through <c>ICityTransportRepository.ExistsAsync</c> before creating one.
/// The last update time comes from <c>AuditableEntity.UpdatedAt</c>.
/// </para>
/// </remarks>
public sealed class CityTransport : AggregateRoot
{
    /// <summary>Maximum length of <see cref="Description"/> and <see cref="SafetyInfo"/>.</summary>
    public const int MaxTextLength = 2000;

    /// <summary>Maximum length of <see cref="OperatingHours"/>.</summary>
    public const int MaxOperatingHoursLength = 200;

    /// <summary>Length of an ISO 4217 currency code.</summary>
    public const int CurrencyCodeLength = 3;

    private readonly List<TransportRoute> _routes = [];
    private readonly List<DestinationTransport> _destinations = [];

    /// <summary>Identifier of the city (a reference to another aggregate, by Id only).</summary>
    public int CityId { get; private set; }

    /// <summary>Identifier of the <see cref="TransportType"/> (a reference to another aggregate, by Id only).</summary>
    public int TransportTypeId { get; private set; }

    /// <summary>General description of the transport option in the city.</summary>
    public string Description { get; private set; } = string.Empty;

    /// <summary>Approximate cost of a typical ride, expressed in <see cref="Currency"/>.</summary>
    public decimal ApproxCost { get; private set; }

    /// <summary>ISO 4217 currency code in upper case (for example <c>EGP</c>).</summary>
    public string Currency { get; private set; } = string.Empty;

    /// <summary>Free-text operating hours (for example <c>05:00 - 24:00</c>).</summary>
    public string OperatingHours { get; private set; } = string.Empty;

    /// <summary>Safety tips and warnings related to using this transport option. May be empty.</summary>
    public string SafetyInfo { get; private set; } = string.Empty;

    /// <summary>Routes offered by this transport option.</summary>
    public IReadOnlyCollection<TransportRoute> Routes => _routes.AsReadOnly();

    /// <summary>Destinations that can be reached by this transport option.</summary>
    public IReadOnlyCollection<DestinationTransport> Destinations => _destinations.AsReadOnly();

    private CityTransport()
    {
    }

    private CityTransport(int cityId, int transportTypeId, Details details)
    {
        CityId = cityId;
        TransportTypeId = transportTypeId;
        Apply(details);
    }

    /// <summary>Creates a city transport after validating its invariants.</summary>
    public static Result<CityTransport> Create(
        int cityId,
        int transportTypeId,
        string? description,
        decimal approxCost,
        string? currency,
        string? operatingHours,
        string? safetyInfo)
    {
        if (cityId <= 0)
        {
            return Result.Failure<CityTransport>(CityTransportErrors.InvalidCityId);
        }

        if (transportTypeId <= 0)
        {
            return Result.Failure<CityTransport>(CityTransportErrors.InvalidTransportTypeId);
        }

        var details = Normalize(description, approxCost, currency, operatingHours, safetyInfo);
        if (details.IsFailure)
        {
            return Result.Failure<CityTransport>(details.Error);
        }

        return new CityTransport(cityId, transportTypeId, details.Value);
    }

    /// <summary>Changes the descriptive details. The city and the transport type never change.</summary>
    public Result<CityTransport> UpdateDetails(
        string? description,
        decimal approxCost,
        string? currency,
        string? operatingHours,
        string? safetyInfo)
    {
        var details = Normalize(description, approxCost, currency, operatingHours, safetyInfo);
        if (details.IsFailure)
        {
            return Result.Failure<CityTransport>(details.Error);
        }

        Apply(details.Value);
        return this;
    }

    /// <summary>Adds a route with its stations. The route needs at least two stations and a unique name.</summary>
    public Result<TransportRoute> AddRoute(
        string? name,
        string? description,
        decimal? fare,
        IReadOnlyCollection<StationInput>? stations)
    {
        var route = TransportRoute.Create(name, description, fare, stations);
        if (route.IsFailure)
        {
            return route;
        }

        if (_routes.Any(r => string.Equals(r.Name, route.Value.Name, StringComparison.OrdinalIgnoreCase)))
        {
            return Result.Failure<TransportRoute>(CityTransportErrors.DuplicateRouteName);
        }

        _routes.Add(route.Value);
        return route;
    }

    /// <summary>Removes a route together with its stations.</summary>
    public Result<CityTransport> RemoveRoute(Guid routeId)
    {
        var route = FindRoute(routeId);
        if (route is null)
        {
            return Result.Failure<CityTransport>(CityTransportErrors.RouteNotFound);
        }

        _routes.Remove(route);
        return this;
    }

    /// <summary>Adds a station to a route.</summary>
    public Result<Station> AddStation(Guid routeId, StationInput? station)
    {
        var route = FindRoute(routeId);
        if (route is null)
        {
            return Result.Failure<Station>(CityTransportErrors.RouteNotFound);
        }

        return route.AddStation(station);
    }

    /// <summary>Removes a station from a route. A route can never drop below two stations.</summary>
    public Result<CityTransport> RemoveStation(Guid routeId, Guid stationId)
    {
        var route = FindRoute(routeId);
        if (route is null)
        {
            return Result.Failure<CityTransport>(CityTransportErrors.RouteNotFound);
        }

        var removed = route.RemoveStation(stationId);
        if (removed.IsFailure)
        {
            return Result.Failure<CityTransport>(removed.Error);
        }

        return this;
    }

    /// <summary>States that this transport reaches a destination.</summary>
    public Result<CityTransport> LinkDestination(Guid destinationId)
    {
        var link = DestinationTransport.Create(destinationId);
        if (link.IsFailure)
        {
            return Result.Failure<CityTransport>(link.Error);
        }

        if (_destinations.Any(d => d.DestinationId == destinationId))
        {
            return Result.Failure<CityTransport>(CityTransportErrors.DuplicateDestination);
        }

        _destinations.Add(link.Value);
        return this;
    }

    /// <summary>States that this transport no longer reaches a destination.</summary>
    public Result<CityTransport> UnlinkDestination(Guid destinationId)
    {
        var link = _destinations.FirstOrDefault(d => d.DestinationId == destinationId);
        if (link is null)
        {
            return Result.Failure<CityTransport>(CityTransportErrors.DestinationNotFound);
        }

        _destinations.Remove(link);
        return this;
    }

    private TransportRoute? FindRoute(Guid routeId) => _routes.FirstOrDefault(r => r.Id == routeId);

    private void Apply(Details details)
    {
        Description = details.Description;
        ApproxCost = details.ApproxCost;
        Currency = details.Currency;
        OperatingHours = details.OperatingHours;
        SafetyInfo = details.SafetyInfo;
    }

    private static Result<Details> Normalize(
        string? description,
        decimal approxCost,
        string? currency,
        string? operatingHours,
        string? safetyInfo)
    {
        var normalizedDescription = description?.Trim() ?? string.Empty;
        if (normalizedDescription.Length == 0 || normalizedDescription.Length > MaxTextLength)
        {
            return Result.Failure<Details>(CityTransportErrors.InvalidDescription);
        }

        if (approxCost < 0m)
        {
            return Result.Failure<Details>(CityTransportErrors.InvalidApproxCost);
        }

        var normalizedCurrency = currency?.Trim().ToUpperInvariant() ?? string.Empty;
        if (normalizedCurrency.Length != CurrencyCodeLength || !normalizedCurrency.All(c => c is >= 'A' and <= 'Z'))
        {
            return Result.Failure<Details>(CityTransportErrors.InvalidCurrency);
        }

        var normalizedHours = operatingHours?.Trim() ?? string.Empty;
        if (normalizedHours.Length == 0 || normalizedHours.Length > MaxOperatingHoursLength)
        {
            return Result.Failure<Details>(CityTransportErrors.InvalidOperatingHours);
        }

        var normalizedSafety = safetyInfo?.Trim() ?? string.Empty;
        if (normalizedSafety.Length > MaxTextLength)
        {
            return Result.Failure<Details>(CityTransportErrors.InvalidSafetyInfo);
        }

        return new Details(normalizedDescription, approxCost, normalizedCurrency, normalizedHours, normalizedSafety);
    }

    private sealed record Details(
        string Description,
        decimal ApproxCost,
        string Currency,
        string OperatingHours,
        string SafetyInfo);
}
