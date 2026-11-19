using Voyara.Application.Abstractions.Authorization;
using Voyara.Application.Abstractions.Messaging;

namespace Voyara.Application.Catalog.Commands.ActivateCategory;

[AuthorizeRequest(Roles.Admin)]
public sealed record ActivateCategoryCommand(Guid Id) : ICommand;