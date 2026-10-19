using Voyara.SharedKernel.Entities;
using Voyara.SharedKernel.Results;

namespace Voyara.Domain.Entities.Transportation;

/// <summary>
/// A named route of a <see cref="CityTransport"/> that passes through at least two <see cref="Station"/>s.
/// Part of the <see cref="CityTransport"/> aggregate: it can only be created and changed through the root.
/// </summary>
public sealed class TransportRoute : BaseEntity
{
    /// <summary>Minimum number of stations a route must have.</summary>
    public const int MinStations = 2;

    /// <summary>Maximum length of <see cref="Name"/>.</summary>
    public const int MaxNameLength = 200;

    /// <summary>Maximum length of <see cref="Description"/>.</summary>
    public const int MaxDescriptionLength = 2000;

    private readonly List<Station> _stations = [];

    /// <summary>Display name of the route.</summary>
    public string Name { get; private set; } = string.Empty;

    /// <summary>Description of the route and useful details for travelers.</summary>
    public string Description { get; private set; } = string.Empty;

    /// <summary>Fare for the whole route, or <see langword="null"/> when it is unknown or variable.</summary>
    public decimal? Fare { get; private set; }

    /// <summary>Stations along the route.</summary>
    public IReadOnlyCollection<Station> Stations => _stations.AsReadOnly();

    private TransportRoute()
    {
    }

    private TransportRoute(string name, string description, decimal? fare)
    {
        Name = name;
        Description = description;
        Fare = fare;
    }

    internal static Result<TransportRoute> Create(
        string? name,
        string? description,
        decimal? fare,
        IReadOnlyCollection<StationInput>? stations)
    {
        var normalizedName = name?.Trim() ?? string.Empty;
        if (normalizedName.Length == 0 || normalizedName.Length > MaxNameLength)
        {
            return Result.Failure<TransportRoute>(CityTransportErrors.InvalidRouteName);
        }

        var normalizedDescription = description?.Trim() ?? string.Empty;
        if (normalizedDescription.Length > MaxDescriptionLength)
        {
            return Result.Failure<TransportRoute>(CityTransportErrors.InvalidRouteDescription);
        }

        if (fare.HasValue && fare.Value < 0m)
        {
            return Result.Failure<TransportRoute>(CityTransportErrors.InvalidFare);
        }

        if (stations is null || stations.Count < MinStations)
        {
            return Result.Failure<TransportRoute>(CityTransportErrors.TooFewStations);
        }

        var route = new TransportRoute(normalizedName, normalizedDescription, fare);

        foreach (var input in stations)
        {
            var station = Station.Create(input);
            if (station.IsFailure)
            {
                return Result.Failure<TransportRoute>(station.Error);
            }

            route._stations.Add(station.Value);
        }

        return route;
    }

    internal Result<Station> AddStation(StationInput? input)
    {
        var station = Station.Create(input);
        if (station.IsFailure)
        {
            return station;
        }

        _stations.Add(station.Value);
        return station;
    }

    internal Result<TransportRoute> RemoveStation(Guid stationId)
    {
        var station = _stations.FirstOrDefault(s => s.Id == stationId);
        if (station is null)
        {
            return Result.Failure<TransportRoute>(CityTransportErrors.StationNotFound);
        }

        if (_stations.Count <= MinStations)
        {
            return Result.Failure<TransportRoute>(CityTransportErrors.TooFewStations);
        }

        _stations.Remove(station);
        return this;
    }
}
