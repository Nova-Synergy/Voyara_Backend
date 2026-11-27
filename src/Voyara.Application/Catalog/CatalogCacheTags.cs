namespace Voyara.Application.Catalog;

/// <summary>Cache tags used to invalidate cached catalog reads when the catalog changes.</summary>
public static class CatalogCacheTags
{
    /// <summary>Every cached destination search result; removed whenever search-visible destination data changes.</summary>
    public const string Destinations = "destinations";
}
