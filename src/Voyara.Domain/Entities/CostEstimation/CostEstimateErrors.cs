using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Voyara.SharedKernel.Results;

namespace Voyara.Domain.Entities.CostEstimation
{
    public static class CostEstimateErrors
    {
        /// <summary>
        /// Gets an error indicating that a trip is required.
        /// </summary>
        public static readonly Error TripRequired =
            Error.Validation("CostEstimate.TripRequired", "Trip is required.");

        /// <summary>
        /// 
        /// </summary>
        public static readonly Error UserRequired =
            Error.Validation("CostEstimate.UserRequired", "User is required.");
        /// <summary>
        /// Gets the validation error indicating that at least one cost line is required.
        /// </summary>
        public static readonly Error LinesRequired =
            Error.Validation("CostEstimate.LinesRequired", "At least one cost line is required.");
        /// <summary>
        /// Error indicating that all cost lines must use the same currency.
        /// </summary>
        public static readonly Error CurrencyMismatch =
            Error.Conflict("CostEstimate.CurrencyMismatch", "All cost lines must use the same currency.");
    }
}
