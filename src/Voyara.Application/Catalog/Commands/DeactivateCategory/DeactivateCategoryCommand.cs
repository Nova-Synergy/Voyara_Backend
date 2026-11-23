using Voyara.Application.Abstractions.Authorization;
using Voyara.Application.Abstractions.Messaging;

namespace Voyara.Application.Catalog.Commands.DeactivateCategory;

[AuthorizeRequest(Roles.Admin)]
public sealed record DeactivateCategoryCommand(Guid Id) : ICommand;