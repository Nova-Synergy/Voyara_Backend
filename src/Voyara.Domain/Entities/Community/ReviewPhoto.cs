using Voyara.SharedKernel.Entities;

namespace Voyara.Domain.Entities.Community;

/// <summary>A photo belonging to a single review aggregate.</summary>
public sealed class ReviewPhoto : BaseEntity
{
    /// <summary>Identifier of the owning review.</summary>
    public Guid ReviewId { get; private set; }

    /// <summary>Location of the stored image.</summary>
    public string Url { get; private set; } = string.Empty;

    private ReviewPhoto() { }

    private ReviewPhoto(Guid reviewId, string url)
    {
        ReviewId = reviewId;
        Url = url.Trim();
    }

    /// <summary>Creates a photo for use by its owning review.</summary>
    internal static ReviewPhoto Create(Guid reviewId, string url) =>
        new(reviewId, url);
}