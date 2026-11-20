using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Voyara.Domain.Enums;

namespace Voyara.Application.DTOs.Guides
{
    /// <summary>
    /// Represents the pricing details for a guide's services.
    /// </summary>
    /// <param name="Unit">The unit used to calculate the guide's price.</param>
    /// <param name="Amount">The price amount for the specified unit.</param>
    /// <param name="Currency">The currency in which the price is expressed.</param>
    public sealed record GuidePricingDto(PricingUnit Unit, decimal Amount, string Currency);

}
