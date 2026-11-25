using Voyara.Application.Abstractions.Messaging;
using Voyara.Application.DTOs;

namespace Voyara.Application.Trips.Queries.GetTripById;

/// <summary>
/// Query to retrieve a trip plan by its unique identifier.
/// </summary>
/// <param name="TripId">The unique identifier of the trip.</param>
public sealed record GetTripByIdQuery(Guid TripId) : IQuery<TripResponse>;
