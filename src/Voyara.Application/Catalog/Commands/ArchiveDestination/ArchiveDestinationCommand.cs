using Voyara.Application.Abstractions.Authorization;
using Voyara.Application.Abstractions.Messaging;

namespace Voyara.Application.Catalog.Commands.ArchiveDestination;

/// <summary>Archives a destination so it is no longer publicly visible. Admin only.</summary>
[AuthorizeRequest(Roles.Admin)]
public sealed record ArchiveDestinationCommand(Guid DestinationId) : ICommand;
