using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Voyara.Domain.Enums
{
    /// <summary>
    /// Defines the possible statuses of a guide request.
    /// </summary>
    public enum RequestStatus
    {
        /// <summary>
        /// The request is waiting for the guide's response.
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
        /// The request has been cancelled by an authorized user.
        /// </summary>
        Cancelled,

        /// <summary>
        /// The request expired before it was accepted or rejected.
        /// </summary>
        Expired,

        /// <summary>
        /// The accepted request has been completed.
        /// </summary>
        Completed
    }

}
