using Voyara.Application.DTOs;
using Voyara.Domain.Entities.Catalog;

namespace Voyara.Application.Catalog.Queries.SearchDestinations;

/// <summary>
/// Maps a destination to its search-result read model. Written by hand (not Mapster) because the
/// displayed text depends on the requested language at runtime.
/// </summary>
internal static class DestinationSummaryMapping
{
    private const int MaxShortDescriptionLength = 160;

    /// <summary>Creates the summary using the translation of <paramref name="languageCode"/> (with fallback).</summary>
    public static DestinationSummaryResponse ToSummary(this Destination destination, string languageCode)
    {
        var translation = destination.GetTranslation(languageCode);
        var cover = destination.Images.FirstOrDefault(image => image.IsCover)
                    ?? destination.Images.OrderBy(image => image.SortOrder).FirstOrDefault();

        return new DestinationSummaryResponse(
            destination.Id,
            translation.Name,
            Shorten(translation.Description),
            destination.CityId,
            destination.CategoryId,
            destination.Location.Latitude,
            destination.Location.Longitude,
            destination.AverageRating,
            destination.ReviewCount,
            destination.PopularityScore,
            cover?.Url);
    }

    private static string? Shorten(string? text)
        => text is { Length: > MaxShortDescriptionLength }
            ? string.Concat(text.AsSpan(0, MaxShortDescriptionLength).TrimEnd(), "...")
            : text;
}
