using Voyara.Application.Abstractions.Authorization;
using Voyara.Application.Abstractions.Messaging;

namespace Voyara.Application.Transportation.Commands.UpdateCityTransport;

/// <summary>Changes the descriptive details of a city transport. City and transport type never change.</summary>
[AuthorizeRequest(Roles.Admin)]
public sealed record UpdateCityTransportCommand(
    Guid Id,
    string Description,
    decimal ApproxCost,
    string Currency,
    string OperatingHours,
    string? SafetyInfo) : ICommand;
