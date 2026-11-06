using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Voyara.Domain.ValueObjects;
using Voyara.SharedKernel.Entities;
using Voyara.SharedKernel.Results;

namespace Voyara.Domain.Entities.CostEstimation
{
    public class CostEstimate : AggregateRoot
    {
        public Guid TripId { get; private set; }
        public Guid UserId { get; private set; }

        public DateTimeOffset CalculatedAt { get; private set; }

        public Money TotalCost { get; private set; } = null!;
        public CostRange? EstimatedRange { get; private set; }

        private readonly List<CostLine> _lines = [];
        public IReadOnlyCollection<CostLine> Lines => _lines.AsReadOnly();

        private CostEstimate() { }
        /// <summary>
        /// Initializes a cost estimate for a trip and user.
        /// </summary>
        /// <param name="tripId">The identifier of the trip.</param>
        /// <param name="userId">The identifier of the user.</param>
        /// <param name="totalCost">The total estimated cost.</param>
        /// <param name="range">The optional estimated cost range.</param>
        /// <param name="lines">The cost lines included in the estimate.</param>
        private CostEstimate(Guid tripId, Guid userId, Money totalCost,
            CostRange? range, IEnumerable<CostLine> lines)
        {
            Id = Guid.NewGuid();
            TripId = tripId;
            UserId = userId;
            CalculatedAt = DateTimeOffset.UtcNow;
            TotalCost = totalCost;
            EstimatedRange = range;
            _lines.AddRange(lines);
        }
        /// <summary>
        /// Creates a new cost estimate for a trip and user.
        /// </summary>
        /// <param name="tripId">The identifier of the trip.</param>
        /// <param name="userId">The identifier of the user.</param>
        /// <param name="lines">The cost lines included in the estimate.</param>
        /// <param name="range">The optional estimated cost range.</param>
        /// <returns>A result indicating the success or failure of the operation.</returns>
        /// <returns></returns>
        public static Result<CostEstimate> Create(
            Guid tripId,
            Guid userId,
            IEnumerable<CostLine> lines,
            CostRange? range = null)
        {
            if (tripId == Guid.Empty)
                return Result.Failure<CostEstimate>(CostEstimateErrors.TripRequired);

            if (userId == Guid.Empty)
                return Result.Failure<CostEstimate>(CostEstimateErrors.UserRequired);

            ArgumentNullException.ThrowIfNull(lines);

            var materialized = lines.ToList();

            if (materialized.Count == 0)
                return Result.Failure<CostEstimate>(CostEstimateErrors.LinesRequired);

            var currency = materialized[0].Amount.Currency;
            if (materialized.Any(x => x.Amount.Currency != currency))
                return Result.Failure<CostEstimate>(CostEstimateErrors.CurrencyMismatch);

            var total = Money.Create(
                materialized.Sum(x => x.Amount.Amount),
                currency);

            var estimate = new CostEstimate(
                tripId,
                userId,
                total,
                range,
                materialized);

            return estimate;
        }
    }
}
