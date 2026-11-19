using Voyara.Application.Abstractions.Authorization;
using Voyara.Application.Abstractions.Messaging;

namespace Voyara.Application.Geography.Commands.UpdateCity;

[AuthorizeRequest(Roles.Admin)]
public sealed record UpdateCityCommand(
    int Id,
    double Latitude,
    double Longitude) : ICommand;