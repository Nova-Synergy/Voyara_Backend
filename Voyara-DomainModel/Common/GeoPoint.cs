using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Voyara_DomainModel.Common
{
    /// <summary>
    /// A value object representing a position on Earth. It has no identity:
    /// two GeoPoints with the same coordinates are the same point.
    /// It is immutable and can only be created with valid coordinates.
    /// </summary>
    public sealed class GeoPoint
    {
        /// <summary>Latitude in degrees, from -90 (south pole) to 90 (north pole).</summary>
        public double Latitude { get; }

        /// <summary>Longitude in degrees, from -180 to 180.</summary>
        public double Longitude { get; }

        /// <summary>Creates a validated geographic point.</summary>
        /// <param name="latitude">Latitude in degrees, between -90 and 90.</param>
        /// <param name="longitude">Longitude in degrees, between -180 and 180.</param>
        /// <exception cref="DomainException">Thrown when a coordinate is out of range.</exception>
        public GeoPoint(double latitude, double longitude)
        {
            if (latitude is < -90 or > 90)
                throw new DomainException("Latitude must be between -90 and 90.");
            if (longitude is < -180 or > 180)
                throw new DomainException("Longitude must be between -180 and 180.");

            Latitude = latitude;
            Longitude = longitude;
        }
    }
}
