using System.ComponentModel.DataAnnotations;
using Voyara.Domain.Entities.Guides;
namespace Voyara.Api.DTOs.Guides;
public class CreateGuideAvailabilityDto 
{
    [EnumDataType(typeof(DayOfWeek))]
    public DayOfWeek Day { get; set; } 
    public TimeOnly StartTime { get; set; } 
    public TimeOnly EndTime { get; set; } 
}

public class GuideAvailabilityReadDto 
{ 
    public Guid Id { get; set; } 
    public string Day { get; set; } = string.Empty; 
    public TimeOnly StartTime { get; set; } 
    public TimeOnly EndTime { get; set; } 
    public static GuideAvailabilityReadDto FromEntity(GuideAvailability item) => new() { Id=item.Id, Day=item.Day.ToString(), StartTime=item.StartTime, EndTime=item.EndTime }; }
