using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Voyara.Application.DTOs.Guides
{
    /// <summary>
    /// Represents the hours during which a guide is available on a specific day of the week.
    /// </summary>
    /// <param name="Day">The day of the week when the guide is available.</param>
    /// <param name="StartTime">The time when the guide's availability begins.</param>
    /// <param name="EndTime">The time when the guide's availability ends.</param>
    public sealed record GuideAvailabilityDto(DayOfWeek Day, TimeOnly StartTime, TimeOnly EndTime);

}
