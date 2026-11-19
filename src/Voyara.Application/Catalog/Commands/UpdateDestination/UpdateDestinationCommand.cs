using Voyara.Application.Abstractions.Authorization;
using Voyara.Application.Abstractions.Messaging;

namespace Voyara.Application.Catalog.Commands.UpdateDestination;

[AuthorizeRequest(Roles.Admin)]
public sealed record UpdateDestinationCommand(
    Guid Id,
    int CityId,
    Guid CategoryId,
    int EstimatedVisitMinutes,
    double Latitude,
    double Longitude) : ICommand;