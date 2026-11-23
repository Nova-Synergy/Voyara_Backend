using Voyara.Application.Abstractions.Messaging;
using Voyara.Domain.Entities.Catalog;

namespace Voyara.Application.Catalog.Queries.GetAllCategories;

public sealed record GetAllCategoriesQuery
    : IQuery<IReadOnlyList<Category>>;