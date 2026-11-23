using Voyara.Application.Abstractions.Messaging;
using Voyara.Application.DTOs;

namespace Voyara.Application.Trips.Queries.GetSharedTripByToken;

/// <summary>
/// Query to access a trip itinerary through an active, unrevoked share token.
/// </summary>
/// <param name="Token">The secure URL share token string.</param>
public sealed record GetSharedTripByTokenQuery(string Token) : IQuery<TripResponse>;
