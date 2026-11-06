using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Voyara.Domain.Enums;
using Voyara.SharedKernel.Entities;

namespace Voyara.Domain.ValueObjects
{
    public sealed class CostLine : AuditableEntity
    {
        public CostCategory Category { get; private set; }
        public string Description { get; private set; } = string.Empty;
        public Money Amount { get; private set; } = null!;

        private CostLine() { }

        public CostLine(CostCategory category, string description, Money amount)
        {
            if (string.IsNullOrWhiteSpace(description))
                throw new ArgumentException("Description is required.");

            ArgumentNullException.ThrowIfNull(amount);

            Id = Guid.NewGuid();
            Category = category;
            Description = description.Trim();
            Amount = amount;
        }
    }
}
