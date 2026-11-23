using Voyara.Application.Abstractions.Messaging;

namespace Voyara.Application.Trips.Commands.RevokeTripShare;

/// <summary>
/// Command to revoke an existing share token for a trip.
/// </summary>
/// <param name="TripId">The unique identifier of the owning trip.</param>
/// <param name="Token">The share token string to revoke.</param>
public sealed record RevokeTripShareCommand(Guid TripId, string Token) : ICommand;
