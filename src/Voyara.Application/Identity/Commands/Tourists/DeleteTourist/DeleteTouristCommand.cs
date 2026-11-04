using Voyara.Application.Abstractions.Authorization;
using Voyara.Application.Abstractions.Messaging;

namespace Voyara.Application.Identity.Commands.Tourists.DeleteTourist;

/// <summary>Soft-deletes a tourist account. Owner or admin.</summary>
[AuthorizeRequest]
public sealed record DeleteTouristCommand(Guid UserId) : ICommand;
