using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Voyara.Domain.Refrence_Class
{
    /// <summary>
    /// Represents a destination referenced by other domain entities.
    /// </summary>
    public class Destination
    {
        /// <summary>
        /// Gets or sets the unique identifier of the destination.
        /// </summary>
        public Guid Id { get; set; }
    }
}
