using Voyara.Application.Abstractions.Messaging;

namespace Voyara.Application.Trips.Commands.DeleteTrip;

/// <summary>
/// Command to remove an existing trip aggregate and its associated shares.
/// </summary>
/// <param name="TripId">The unique identifier of the trip to delete.</param>
public sealed record DeleteTripCommand(Guid TripId) : ICommand;
