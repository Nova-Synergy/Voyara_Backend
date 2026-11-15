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

    /// <summary>One-based page number.</summary>
    public int Page { get; init; } = 1;

    /// <summary>Items per page.</summary>
    public int PageSize { get; init; } = 10;
}
