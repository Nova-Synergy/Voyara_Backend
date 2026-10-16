using Voyara.Domain.ValueObjects;
using Voyara.SharedKernel.Entities;
using Voyara.SharedKernel.Results;

namespace Voyara.Domain.Entities.Transportation;

/// <summary>
/// A stop or station on a <see cref="TransportRoute"/>. Part of the <see cref="CityTransport"/> aggregate:
/// it can only be created and changed through the aggregate root.
/// </summary>
public sealed class Station : BaseEntity
{
    /// <summary>Maximum length of <see cref="Name"/>.</summary>
    public const int MaxNameLength = 200;

    /// <summary>Display name of the station.</summary>
    public string Name { get; private set; } = string.Empty;

    /// <summary>Geographic location of the station.</summary>
    public GeoPoint Location { get; private set; } = null!;

    private Station()
    {
    }

    private Station(string name, GeoPoint location)
    {
        Name = name;
        Location = location;
    }

    internal static Result<Station> Create(StationInput? input)
    {
        var name = input?.Name?.Trim() ?? string.Empty;
        if (name.Length == 0 || name.Length > MaxNameLength)
        {
            return Result.Failure<Station>(CityTransportErrors.InvalidStationName);
        }

        if (input?.Location is null)
        {
            return Result.Failure<Station>(CityTransportErrors.InvalidStationLocation);
        }

        return new Station(name, input.Location);
    }
}
