using Voyara.Application.Abstractions.Authorization;
using Voyara.Application.Abstractions.Messaging;

namespace Voyara.Application.Geography.Commands.DeactivateCity;

[AuthorizeRequest(Roles.Admin)]
public sealed record DeactivateCityCommand(int Id) : ICommand;