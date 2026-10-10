using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Voyara.Domain.Enums
{
    /// <summary>
    /// Defines the possible lifecycle states of a guide request.
    /// </summary>
    public enum RequestStatus
    {
        /// <summary>
        /// The request is awaiting a decision from the guide.
        /// </summary>
        Pending,

        /// <summary>
        /// The guide has accepted the request.
        /// </summary>
        Accepted,

        /// <summary>
        /// The guide has rejected the request.
        /// </summary>
        Rejected,

        /// <summary>
        /// The request has been cancelled.
        /// </summary>
        Cancelled,

        /// <summary>
        /// The request has expired without being accepted.
        /// </summary>
        Expired,

        /// <summary>
        /// The tour associated with the request has been completed.
        /// </summary>
        Completed
    }
}
