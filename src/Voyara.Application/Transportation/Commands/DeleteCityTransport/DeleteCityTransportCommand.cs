using Voyara.Application.Abstractions.Messaging;

namespace Voyara.Application.Transportation.Commands.DeleteCityTransport;

/// <summary>Deletes a city transport (soft delete).</summary>
public sealed record DeleteCityTransportCommand(Guid CityTransportId) : ICommand;
