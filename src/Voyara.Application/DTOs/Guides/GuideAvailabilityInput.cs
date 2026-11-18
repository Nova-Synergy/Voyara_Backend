using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Voyara.Application.DTOs.Guides
{
    /// <summary>A recurring weekly availability range (request input).</summary>
    public sealed record GuideAvailabilityInput(DayOfWeek Day, TimeOnly StartTime, TimeOnly EndTime);
}
