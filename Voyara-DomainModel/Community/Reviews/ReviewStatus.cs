using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Voyara_DomainModel.Community.Reviews
{
    /// <summary>The moderation lifecycle of a <see cref="Review"/>.</summary>
    public enum ReviewStatus
    {
        /// <summary>Submitted by the tourist and waiting for an admin decision.</summary>
        Pending,

        /// <summary>Accepted by an admin and visible to everyone.</summary>
        Approved,

        /// <summary>Refused by an admin. The tourist is told why.</summary>
        Rejected,

        /// <summary>An approved review that users have reported and that awaits investigation.</summary>
        Reported,

        /// <summary>Removed from public view by an admin.</summary>
        Hidden
    }
}
