using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Voyara.Domain.Entities.Culture_Rules;

namespace Voyara.Domain.Enums
{

    /// <summary>
    /// Defines the geographic level to which a local rule applies.
    /// </summary>
    public enum ScopeType
    {
        /// <summary>The rule applies to a whole country.</summary>
        Country,

        /// <summary>The rule applies to a city.</summary>
        City,

        /// <summary>The rule applies to a single destination.</summary>
        Destination
    }

}
