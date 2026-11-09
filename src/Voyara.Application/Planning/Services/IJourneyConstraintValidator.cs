using Voyara.Application.DTOs;
using Voyara.Domain.Entities.TripAndJourney;

namespace Voyara.Application.Planning.Services;

/// <summary>
/// Service contract for validating journey business invariants, daily capacity, and scheduling feasibility.
/// </summary>
public interface IJourneyConstraintValidator
{
    /// <summary>
    /// Evaluates a journey aggregate against domain invariants and temporal/capacity constraints.
    /// </summary>
    /// <param name="journey">The journey aggregate root to validate.</param>
    /// <returns>A comprehensive <see cref="JourneyValidationResponse"/> report.</returns>
    JourneyValidationResponse Validate(Journey journey);
}
