using Voyara.Application.Abstractions.Messaging;
using Voyara.Application.DTOs;

namespace Voyara.Application.Trips.Commands.ShareTrip;

/// <summary>
/// Command to create a shareable access token for a trip.
/// </summary>
/// <param name="TripId">The unique identifier of the trip to share.</param>
/// <param name="ExpiresAt">Optional expiration date and time in UTC.</param>
public sealed record ShareTripCommand(
    Guid TripId,
    DateTime? ExpiresAt = null) : ICommand<TripShareResponse>;
