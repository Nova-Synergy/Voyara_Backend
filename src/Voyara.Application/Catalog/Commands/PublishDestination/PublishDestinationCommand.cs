using Voyara.Application.Abstractions.Authorization;
using Voyara.Application.Abstractions.Messaging;

namespace Voyara.Application.Catalog.Commands.PublishDestination;

/// <summary>Publishes a draft destination so it becomes publicly visible. Admin only.</summary>
[AuthorizeRequest(Roles.Admin)]
public sealed record PublishDestinationCommand(Guid DestinationId) : ICommand;
