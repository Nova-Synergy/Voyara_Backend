using System.ComponentModel.DataAnnotations;
using Voyara.Domain.Entities.Guides;
namespace Voyara.Api.DTOs.Guides;
public class CreateGuideLanguageDto 
{
    [Required, StringLength(10)]
    public string LanguageCode { get; set; } = string.Empty;

    [Required, StringLength(50)] 
    public string Proficiency { get; set; } = string.Empty; 
}
public class GuideLanguageReadDto 
{ 
    public Guid Id { get; set; } 
    public string LanguageCode { get; set; } = string.Empty; 
    public string Proficiency { get; set; } = string.Empty; 
    public static GuideLanguageReadDto FromEntity(GuideLanguage item) => new() { Id=item.Id, LanguageCode=item.LanguageCode, Proficiency=item.Proficiency }; }
