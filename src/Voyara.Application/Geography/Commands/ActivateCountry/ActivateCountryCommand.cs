using Voyara.Application.Abstractions.Authorization;
using Voyara.Application.Abstractions.Messaging;

namespace Voyara.Application.Geography.Commands.ActivateCountry;

[AuthorizeRequest(Roles.Admin)]
public sealed record ActivateCountryCommand(int Id) : ICommand;