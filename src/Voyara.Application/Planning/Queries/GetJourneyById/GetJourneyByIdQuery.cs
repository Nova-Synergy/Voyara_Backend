using Voyara.Application.Abstractions.Messaging;
using Voyara.Application.DTOs;

namespace Voyara.Application.Planning.Queries.GetJourneyById;

/// <summary>
/// Query to retrieve a single journey itinerary by its unique identifier.
/// </summary>
/// <param name="JourneyId">The unique identifier of the journey to retrieve.</param>
public sealed record GetJourneyByIdQuery(Guid JourneyId) : IQuery<JourneyResponse>;
