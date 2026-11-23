using Voyara.Application.Abstractions.Authorization;
using Voyara.Application.Abstractions.Messaging;

namespace Voyara.Application.Transportation.Commands.DeleteCityTransport;

/// <summary>Soft-deletes a city transport (the audit interceptor turns the delete into a soft delete).</summary>
[AuthorizeRequest(Roles.Admin)]
public sealed record DeleteCityTransportCommand(Guid Id) : ICommand;
