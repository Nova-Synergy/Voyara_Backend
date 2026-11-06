using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Voyara.SharedKernel.ValueObjects;

namespace Voyara.Domain.ValueObjects
{
    public sealed class CostRange : ValueObject
    {
        public Money Minimum { get; }
        public Money Maximum { get; }

        private CostRange(Money minimum, Money maximum)
        {
            Minimum = minimum;
            Maximum = maximum;
        }
        /// <summary>
        /// Creates a cost range from its minimum and maximum values.
        /// </summary>
        /// <param name="minimum">The minimum cost in the range.</param>
        /// <param name="maximum">The maximum cost in the range.</param>
        /// <returns>A cost range containing the specified minimum and maximum.</returns>
        /// <exception cref="ArgumentException">The currencies differ, or the minimum amount exceeds the maximum amount.</exception>
        public static CostRange Create(Money minimum, Money maximum)
        {
            ArgumentNullException.ThrowIfNull(minimum);
            ArgumentNullException.ThrowIfNull(maximum);

            if (minimum.Currency != maximum.Currency)
                throw new ArgumentException("Currencies must match.");

            if (minimum.Amount > maximum.Amount)
                throw new ArgumentException("Minimum cannot exceed maximum.");

            return new(minimum, maximum);
        }
/// <summary>
/// Defines the values used to determine equality.
/// </summary>
/// <returns>The minimum and maximum values.</returns>
        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return Minimum;
            yield return Maximum;
        }
    }
}
