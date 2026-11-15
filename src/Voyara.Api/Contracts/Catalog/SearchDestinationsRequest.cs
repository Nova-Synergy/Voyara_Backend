namespace Voyara.Api.Contracts.Catalog;

/// <summary>Query-string parameters of the destination search endpoint.</summary>
public sealed record SearchDestinationsRequest
{
    /// <summary>Text matched against destination names and descriptions.</summary>
    public string? SearchTerm { get; init; }

    /// <summary>Language of the returned texts (for example en or ar); defaults to English.</summary>
    public string? Lang { get; init; }

    /// <summary>Only destinations of cities in this country.</summary>
    public int? CountryId { get; init; }

    /// <summary>Only destinations in this city.</summary>
    public int? CityId { get; init; }

    /// <summary>Only destinations of this category.</summary>
    public Guid? CategoryId { get; init; }

    /// <summary>Minimum average rating (0 to 5).</summary>
    public double? MinRating { get; init; }

    /// <summary>Maximum ticket price, expressed in <see cref="Currency"/>.</summary>
    public decimal? MaxPrice { get; init; }

    /// <summary>ISO 4217 currency code of <see cref="MaxPrice"/> (for example EGP); required with a price limit.</summary>
    public string? Currency { get; init; }

    /// <summary>One-based page number.</summary>
    public int Page { get; init; } = 1;

    /// <summary>Items per page.</summary>
    public int PageSize { get; init; } = 10;
}
