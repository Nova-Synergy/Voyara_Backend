using Voyara.SharedKernel.Entities;

namespace Voyara.Domain.Entities.Community;

/// <summary>
/// A photo attached to a <see cref="Review"/>. It only exists as part of its review,
/// so it can only be created through the review (the factory is internal).
/// </summary>
public sealed class ReviewPhoto : BaseEntity
{
    /// <summary>Identifier of the review this photo belongs to.</summary>
    public Guid ReviewId { get; private set; }

    /// <summary>Location of the stored image.</summary>
    public string Url { get; private set; } = string.Empty;

    private ReviewPhoto()
    {
    }

    private ReviewPhoto(string url) => Url = url;

    internal static ReviewPhoto Create(string url) => new(url.Trim());
}