using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Voyara_DomainModel.Community.PlaceSuggestions
{
    /// <summary>The decision state of a <see cref="PlaceSuggestion"/>.</summary>
    public enum SuggestionStatus
    {
        /// <summary>Waiting for an admin decision.</summary>
        Pending,

        /// <summary>Accepted by an admin; it can become a destination.</summary>
        Approved,

        /// <summary>Refused by an admin.</summary>
        Rejected
    }
}
