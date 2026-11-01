using System.ComponentModel.DataAnnotations;
using Voyara.Domain.Entities.Guides;

namespace Voyara.Api.DTOs.Guides;

public class CreateGuideTimeOffDto
{
    public DateOnly Date { get; set; }

    [StringLength(300)]
    public string? Reason { get; set; }
}

public class GuideTimeOffReadDto
{
    public Guid Id { get; set; }
    public DateOnly Date { get; set; }
    public string? Reason { get; set; }

    public static GuideTimeOffReadDto FromEntity(GuideTimeOff timeOff) => new()
    {
        Id = timeOff.Id,
        Date = timeOff.Date,
        Reason = timeOff.Reason
    };
}
