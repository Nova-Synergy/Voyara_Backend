using Voyara.Domain.Enums;

namespace Voyara.Api.Contracts.Catalog;

/// <summary>Query-string parameters of the destination search endpoint.</summary>
public sealed record SearchDestinationsRequest
{
    /// <summary>Text matched against destination names and descriptions.</summary>
    public string? SearchTerm { get; init; }

    /// <summary>Language of the returned texts (for example en or ar); defaults to English.</summary>
    public string? Lang { get; init; }

    /// <summary>Only destinations of cities in this country.</summary>
    public Guid? CountryId { get; init; }

    /// <summary>Only destinations in this city.</summary>
    public Guid? CityId { get; init; }

    /// <summary>Only destinations of this category.</summary>
    public Guid? CategoryId { get; init; }

    /// <summary>Minimum average rating (0 to 5).</summary>
    public double? MinRating { get; init; }

    /// <summary>Maximum ticket price, expressed in <see cref="Currency"/>.</summary>
    public decimal? MaxPrice { get; init; }

    /// <summary>ISO 4217 currency code of <see cref="MaxPrice"/> (for example EGP); required with a price limit.</summary>
    public string? Currency { get; init; }

    /// <summary>Sort key: Popularity (default), Rating, Name or Newest.</summary>
    public DestinationSortBy SortBy { get; init; } = DestinationSortBy.Popularity;

    /// <summary>Ascending or Descending; omitted uses the natural direction of the sort key.</summary>
    public SortDirection? SortDirection { get; init; }

    /// <summary>One-based page number.</summary>
    public int Page { get; init; } = 1;

    /// <summary>Items per page (1 to 100).</summary>
    public int PageSize { get; init; } = 10;
}
