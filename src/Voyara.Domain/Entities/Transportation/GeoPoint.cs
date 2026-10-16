using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Voyara.Domain.Entities.Transportation
{
    /// <summary>
    /// Value object that represents a geographic coordinate (WGS 84).
    /// </summary>
    /// <remarks>
    /// Mapped as an EF Core owned type, so its values are stored as the <c>Latitude</c> and <c>Longitude</c>
    /// columns of the owning entity's table (see <see cref="Station.Location"/>).
    /// </remarks>
    /// <param name="Latitude">Latitude in decimal degrees, from -90 to 90.</param>
    /// <param name="Longitude">Longitude in decimal degrees, from -180 to 180.</param>
    public sealed record GeoPoint(double Latitude, double Longitude);

}
