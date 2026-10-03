using Voyara.Application.Abstractions.Messaging;

namespace Voyara.Application.Catalog.Commands.CreateDestination;

/// <summary>Creates a destination in Draft status; returns the destination identifier.</summary>
public sealed record CreateDestinationCommand(string Name, string? Description, double Latitude, double Longitude)
    : ICommand<Guid>;
