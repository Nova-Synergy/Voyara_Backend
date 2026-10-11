using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Voyara.Domain.Entities.Culture_Rules;

namespace Voyara.Domain.Enums
{
    /// <summary>
    /// Categorizes a <see cref="LocalRule"/> by the kind of behavior or restriction it describes.
    /// </summary>
    public enum RuleType
    {
        /// <summary>A rule that does not fit any other category.</summary>
        Custom,

        /// <summary>Rules about taking photos or videos (for example, restricted subjects or places).</summary>
        Photography,

        /// <summary>Rules about clothing expected at a place or in a region.</summary>
        DressCode,

        /// <summary>Rules related to religious practice, sites or customs.</summary>
        Religious,

        /// <summary>Areas that are closed or require special permission to enter.</summary>
        RestrictedArea,

        /// <summary>Legal requirements and prohibitions that travelers must follow.</summary>
        LocalLaw,

        /// <summary>Expected social behavior and etiquette.</summary>
        Behavior,

        /// <summary>Safety-related rules and precautions.</summary>
        Safety
    }
}
