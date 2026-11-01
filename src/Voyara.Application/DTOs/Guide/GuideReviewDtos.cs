using System.ComponentModel.DataAnnotations;
using Voyara.Domain.Entities.Guides;

namespace Voyara.Api.DTOs.Guides;

public class CreateGuideReviewDto
{
    [Range(1, 5)]
    public int Rating { get; set; }

    [Required]
    [StringLength(2000)]
    public string Text { get; set; } = string.Empty;
}

public class GuideReviewReadDto
{
    public Guid Id { get; set; }
    public Guid GuideRequestId { get; set; }
    public Guid GuideProfileId { get; set; }
    public int Rating { get; set; }
    public string Text { get; set; } = string.Empty;
    public DateTimeOffset? CreatedAt { get; set; }

    public static GuideReviewReadDto FromEntity(GuideReview review) => new()
    {
        Id = review.Id,
        GuideRequestId = review.GuideRequestId,
        GuideProfileId = review.GuideProfileId,
        Rating = review.Rating,
        Text = review.Text,
        CreatedAt = review.CreatedAt
    };
}
