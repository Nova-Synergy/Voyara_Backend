using Voyara.Application.Abstractions.Authorization;
using Voyara.Application.Abstractions.Messaging;

namespace Voyara.Application.Geography.Commands.CreateCountry;

[AuthorizeRequest(Roles.Admin)]
public sealed record CreateCountryCommand(
    string IsoCode,
    string DefaultCurrency,
    string TimeZone) : ICommand<int>;