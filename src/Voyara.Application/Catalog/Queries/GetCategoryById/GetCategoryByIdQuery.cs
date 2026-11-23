using Voyara.Application.Abstractions.Messaging;
using Voyara.Domain.Entities.Catalog;

namespace Voyara.Application.Catalog.Queries.GetCategoryById;

public sealed record GetCategoryByIdQuery(Guid Id)
    : IQuery<Category>;