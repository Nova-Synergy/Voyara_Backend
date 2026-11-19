using Voyara.Application.Abstractions.Authorization;
using Voyara.Application.Abstractions.Messaging;

namespace Voyara.Application.Geography.Commands.ActivateCity;

[AuthorizeRequest(Roles.Admin)]
public sealed record ActivateCityCommand(int Id) : ICommand;