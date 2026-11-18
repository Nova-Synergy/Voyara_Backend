using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Voyara.Domain.Enums;

namespace Voyara.Application.DTOs.Guides
{
    /// <summary>A pricing option (request input).</summary>
    public sealed record GuidePricingInput(PricingUnit Unit, decimal Amount, string Currency);
}
