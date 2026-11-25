using Voyara.Application.Abstractions.Authorization;
using Voyara.Application.Abstractions.Messaging;

namespace Voyara.Application.Catalog.Commands.CreateDestination;

[AuthorizeRequest(Roles.Admin)]
public sealed record CreateDestinationCommand(
    int CityId,
    Guid CategoryId,
    int EstimatedVisitMinutes,
    string LanguageCode,
    string Name,
    string? Description,
    string? CultureNotes,
    double Latitude,
    double Longitude) : ICommand<Guid>;