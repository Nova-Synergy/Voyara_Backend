using Voyara.SharedKernel.Entities;

namespace Voyara.Domain.Entities.Community;

/// <summary>A photo belonging to a single place suggestion.</summary>
public sealed class SuggestionPhoto : AuditableEntity
{
    /// <summary>Identifier of the owning place suggestion.</summary>
    public Guid PlaceSuggestionId { get; private set; }

    /// <summary>Location of the stored image.</summary>
    public string Url { get; private set; } = string.Empty;

    private SuggestionPhoto() { }

    private SuggestionPhoto(Guid placeSuggestionId, string url)
    {
        PlaceSuggestionId = placeSuggestionId;
        Url = url.Trim();
    }

    /// <summary>Creates a photo for use by its owning suggestion.</summary>
    internal static SuggestionPhoto Create(Guid placeSuggestionId, string url) =>
        new(placeSuggestionId, url);
}