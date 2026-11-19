using Voyara.Application.Abstractions.Messaging;
using Voyara.Domain.Entities.Catalog;

namespace Voyara.Application.Catalog.Queries.GetAllCategories;

public sealed record GetAllCategoriesQuery(
    bool? IsActive = null,
    string? Search = null) : IQuery<IReadOnlyList<Category>>;