using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Voyara_DomainModel.Common
{

    /// <summary>
    /// Thrown when an operation would break a business rule, for example approving
    /// a review that was already rejected. The API layer can catch it and turn it
    /// into a clean error response (such as HTTP 400 or 409).
    /// </summary>
    public class DomainException : Exception
    {
        /// <summary>Creates the exception with a message describing the broken rule.</summary>
        /// <param name="message">A human-readable explanation of the broken rule.</param>
        public DomainException(string message) : base(message) { }
    }
}
