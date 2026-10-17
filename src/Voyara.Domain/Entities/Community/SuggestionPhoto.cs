using Voyara.SharedKernel.Entities;

namespace Voyara.Domain.Entities.Community;

/// <summary>
/// A photo attached to a <see cref="PlaceSuggestion"/>. It only exists as part of its suggestion,
/// so it can only be created through the suggestion (the factory is internal).
/// </summary>
public sealed class SuggestionPhoto : AuditableEntity
{
    /// <summary>Identifier of the suggestion this photo belongs to.</summary>
    public Guid PlaceSuggestionId { get; private set; }

    /// <summary>Location of the stored image.</summary>
    public string Url { get; private set; } = string.Empty;

    private SuggestionPhoto()
    {
    }

    private SuggestionPhoto(string url) => Url = url;

    internal static SuggestionPhoto Create(string url) => new(url.Trim());
}