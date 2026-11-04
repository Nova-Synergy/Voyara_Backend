using System.ComponentModel.DataAnnotations;
using Voyara.Domain.Entities.Guides;
using Voyara.Domain.Enums;
namespace Voyara.Api.DTOs.Guides;

public class CreateGuidePricingDto 
{ 
    [EnumDataType(typeof(PricingUnit))] 
    public PricingUnit Unit { get; set; } 

    [Range(0, double.MaxValue)] 
    public decimal Amount { get; set; } 

    [Required, StringLength(3, MinimumLength=3)] 
    public string Currency { get; set; } = string.Empty; 
}
public class GuidePricingReadDto 
{ 
    public Guid Id { get; set; } 
    public string Unit { get; set; } = string.Empty; 
    public decimal Amount { get; set; } 
    public string Currency { get; set; } = string.Empty; 
    public static GuidePricingReadDto FromEntity(GuidePricing item) => new() { Id=item.Id, Unit=item.Unit.ToString(), Amount=item.Amount, Currency=item.Currency };
}
