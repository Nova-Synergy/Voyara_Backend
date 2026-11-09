using Voyara.Application.Abstractions.Messaging;
using Voyara.Application.DTOs;

namespace Voyara.Application.Planning.Queries.ValidateJourney;

/// <summary>
/// Query to assess domain invariants, temporal overlaps, and capacity constraints of a journey.
/// </summary>
/// <param name="JourneyId">The unique identifier of the journey to validate.</param>
public sealed record ValidateJourneyQuery(Guid JourneyId) : IQuery<JourneyValidationResponse>;
