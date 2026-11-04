using System.ComponentModel.DataAnnotations;
using Voyara.Domain.Entities.Guides;
namespace Voyara.Api.DTOs.Guides;

public class CreateGuideExpertiseDto 
{ 
    [Range(1, int.MaxValue)] 
    public int CategoryId { get; set; }
}
public class GuideExpertiseReadDto 
{ 
    public Guid Id { get; set; } 
    public int CategoryId { get; set; } 
    public static GuideExpertiseReadDto FromEntity(GuideExpertise item) => new() { Id = item.Id, CategoryId = item.CategoryId }; 
}
