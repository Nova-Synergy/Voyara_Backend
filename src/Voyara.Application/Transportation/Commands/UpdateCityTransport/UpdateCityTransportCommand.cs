using Voyara.Application.Abstractions.Messaging;

namespace Voyara.Application.Transportation.Commands.UpdateCityTransport;

/// <summary>Changes the descriptive details of a city transport. The city and the transport type never change.</summary>
public sealed record UpdateCityTransportCommand(
    Guid CityTransportId,
    string Description,
    decimal ApproxCost,
    string Currency,
    string OperatingHours,
    string? SafetyInfo) : ICommand;
