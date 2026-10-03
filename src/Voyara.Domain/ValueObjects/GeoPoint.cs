using Voyara.SharedKernel.Results;
using Voyara.SharedKernel.ValueObjects;

namespace Voyara.Domain.ValueObjects;

/// <summary>Geographic coordinate (WGS84) used for destination locations.</summary>
public sealed class GeoPoint : ValueObject
{
    private const double MinLatitude = -90.0;
    private const double MaxLatitude = 90.0;
    private const double MinLongitude = -180.0;
    private const double MaxLongitude = 180.0;

    /// <summary>Latitude in degrees.</summary>
    public double Latitude { get; private set; }

    /// <summary>Longitude in degrees.</summary>
    public double Longitude { get; private set; }

    private GeoPoint()
    {
    }

    private GeoPoint(double latitude, double longitude)
    {
        Latitude = latitude;
        Longitude = longitude;
    }

    /// <summary>Creates a validated geographic coordinate.</summary>
    public static Result<GeoPoint> Create(double latitude, double longitude)
    {
        if (latitude < MinLatitude || latitude > MaxLatitude)
        {
            return Result.Failure<GeoPoint>(Error.Validation(
                "GeoPoint.InvalidLatitude", "Latitude must be between -90 and 90 degrees."));
        }

        if (longitude < MinLongitude || longitude > MaxLongitude)
        {
            return Result.Failure<GeoPoint>(Error.Validation(
                "GeoPoint.InvalidLongitude", "Longitude must be between -180 and 180 degrees."));
        }

        return new GeoPoint(latitude, longitude);
    }

    /// <inheritdoc />
    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Latitude;
        yield return Longitude;
    }
}
