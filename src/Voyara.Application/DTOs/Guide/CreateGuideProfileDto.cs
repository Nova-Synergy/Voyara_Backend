using System.ComponentModel.DataAnnotations;

namespace Voyara.Api.DTOs.Guides;

/// <summary>Data needed to create a guide profile. The owner is the signed-in user.</summary>
public class CreateGuideProfileDto
{
    [Range(1, int.MaxValue)]
    public int CityId { get; set; }

    [Required]
    [StringLength(1000)]
    public string Bio { get; set; } = string.Empty;

    [StringLength(2048)]
    public string? PhotoUrl { get; set; }

    [Range(0, int.MaxValue)]
    public int YearsOfExperience { get; set; }
}
