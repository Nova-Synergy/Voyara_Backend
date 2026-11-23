using Voyara.Application.Abstractions.Authorization;
using Voyara.Application.Abstractions.Messaging;

namespace Voyara.Application.Catalog.Commands.UpdateCategory;

[AuthorizeRequest(Roles.Admin)]
public sealed record UpdateCategoryCommand(
    Guid Id,
    string Icon,
    bool IsActive) : ICommand;