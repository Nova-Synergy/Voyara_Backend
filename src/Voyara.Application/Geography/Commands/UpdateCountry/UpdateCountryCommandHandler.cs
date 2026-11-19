using Voyara.Application.Abstractions.Authorization;
using Voyara.Application.Abstractions.Messaging;

namespace Voyara.Application.Geography.Commands.UpdateCountry;

[AuthorizeRequest(Roles.Admin)]
public sealed record UpdateCountryCommand(
    int Id,
    string DefaultCurrency,
    string TimeZone) : ICommand;