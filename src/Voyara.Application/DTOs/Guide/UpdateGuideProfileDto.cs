using System.ComponentModel.DataAnnotations;
namespace Voyara.Api.DTOs.Guides;
public class UpdateGuideProfileDto
{
    [Required, StringLength(1000)]
    public string Bio { get; set; } = string.Empty;
    
    [StringLength(2048)] 
    public string? PhotoUrl { get; set; } 
    
    [Range(0, int.MaxValue)] 
    public int YearsOfExperience { get; set; } 
}
