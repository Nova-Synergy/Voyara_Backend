using Voyara.Application.Abstractions.Messaging;

namespace Voyara.Application.Transportation.Commands.CreateCityTransport;

/// <summary>Creates the transport option of a transport type in a city; returns the city transport identifier.</summary>
public sealed record CreateCityTransportCommand(
    int CityId,
    int TransportTypeId,
    string Description,
    decimal ApproxCost,
    string Currency,
    string OperatingHours,
    string? SafetyInfo) : ICommand<Guid>;
