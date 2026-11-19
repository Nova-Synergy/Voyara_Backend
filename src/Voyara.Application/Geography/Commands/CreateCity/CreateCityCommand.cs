using Voyara.Application.Abstractions.Authorization;
using Voyara.Application.Abstractions.Messaging;

namespace Voyara.Application.Geography.Commands.CreateCity;

[AuthorizeRequest(Roles.Admin)]
public sealed record CreateCityCommand(
    int CountryId,
    double Latitude,
    double Longitude) : ICommand<int>;