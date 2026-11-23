using Voyara.Application.Abstractions.Authorization;
using Voyara.Application.Abstractions.Messaging;

namespace Voyara.Application.Transportation.Commands.CreateCityTransport;

/// <summary>Creates the transport information of one transport type in one city. Returns the new identifier.</summary>
[AuthorizeRequest(Roles.Admin)]
public sealed record CreateCityTransportCommand(
    Guid CityId,
    int TransportTypeId,
    string Description,
    decimal ApproxCost,
    string Currency,
    string OperatingHours,
    string? SafetyInfo) : ICommand<Guid>;
