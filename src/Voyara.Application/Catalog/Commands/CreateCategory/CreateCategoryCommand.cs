using Voyara.Application.Abstractions.Authorization;
using Voyara.Application.Abstractions.Messaging;

namespace Voyara.Application.Catalog.Commands.CreateCategory;

[AuthorizeRequest(Roles.Admin)]
public sealed record CreateCategoryCommand(
    string Code,
    string Icon) : ICommand<Guid>;