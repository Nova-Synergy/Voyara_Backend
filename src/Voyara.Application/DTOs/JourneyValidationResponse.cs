namespace Voyara.Application.DTOs;

/// <summary>
/// Detail of a specific scheduling or capacity constraint violation within a journey.
/// </summary>
public sealed record JourneyConstraintViolationResponse(
    int? DayNumber,
    Guid? ItemId,
    string ConstraintType,
    string Description);

/// <summary>
/// Comprehensive validation and feasibility assessment for a journey itinerary.
/// </summary>
public sealed record JourneyValidationResponse(
    Guid JourneyId,
    bool IsValid,
    IReadOnlyList<string> InvariantErrors,
    IReadOnlyList<JourneyConstraintViolationResponse> ConstraintViolations);
