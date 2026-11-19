using Voyara.Application.Abstractions.Authorization;
using Voyara.Application.Abstractions.Messaging;

namespace Voyara.Application.Geography.Commands.DeactivateCountry;

[AuthorizeRequest(Roles.Admin)]
public sealed record DeactivateCountryCommand(int Id) : ICommand;