using Voyara.Application.DTOs;
using Voyara.Domain.Entities.Catalog;
using Voyara.Domain.Entities.Localization;

namespace Voyara.Application.Catalog.Queries.SearchDestinations;

/// <summary>
/// Maps a destination to its search-result read model. Written by hand (not Mapster) because the displayed text
/// depends on the requested language at runtime.
/// </summary>
public static class DestinationSummaryMapping
{
    private const string FallbackLanguage = SearchDestinationsQuery.DefaultLanguage;

    /// <summary>
    /// Creates the summary using the translation chosen by <see cref="SelectTranslation"/>; returns null when the
    /// destination has no translation at all (it cannot be displayed).
    /// </summary>
    public static DestinationSummaryResponse? ToSummary(this Destination destination, string languageCode)
    {
        var translation = SelectTranslation(destination.Translations, languageCode);
        if (translation is null)
        {
            return null;
        }

        var cover = destination.Images.FirstOrDefault(image => image.IsCover)
                    ?? destination.Images.OrderBy(image => image.SortOrder).FirstOrDefault();

        return new DestinationSummaryResponse(
            destination.Id,
            translation.Name,
            translation.Description,
            destination.Location.Latitude,
            destination.Location.Longitude,
            destination.Status.ToString(),
            destination.CityId,
            destination.CategoryId,
            destination.AverageRating,
            destination.ReviewCount,
            destination.PopularityScore,
            cover?.Url);
    }

    /// <summary>
    /// Picks the translation to display, in this exact order: (1) the requested language, (2) English,
    /// (3) the translation with the lowest language code (ordinal). The last rule makes the choice deterministic
    /// regardless of load order and matches the translation selection used by destination name sorting.
    /// Returns null for an empty list.
    /// </summary>
    public static DestinationTranslation? SelectTranslation(
        IEnumerable<DestinationTranslation> translations,
        string languageCode)
    {
        var list = translations as IReadOnlyCollection<DestinationTranslation> ?? translations.ToList();

        return list.FirstOrDefault(t => string.Equals(t.LanguageCode, languageCode, StringComparison.OrdinalIgnoreCase))
               ?? list.FirstOrDefault(t => string.Equals(t.LanguageCode, FallbackLanguage, StringComparison.OrdinalIgnoreCase))
               ?? list.OrderBy(t => t.LanguageCode, StringComparer.Ordinal).FirstOrDefault();
    }
}
