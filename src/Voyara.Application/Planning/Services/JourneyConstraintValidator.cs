using Voyara.Application.DTOs;
using Voyara.Domain.Entities.TripAndJourney;

namespace Voyara.Application.Planning.Services;

/// <summary>
/// Implements domain invariant and scheduling constraint checks across journey itineraries.
/// </summary>
public sealed class JourneyConstraintValidator : IJourneyConstraintValidator
{
    private const int MaxStopsPerDay = 6;

    /// <inheritdoc />
    public JourneyValidationResponse Validate(Journey journey)
    {
        ArgumentNullException.ThrowIfNull(journey);

        var invariantErrors = new List<string>();
        var violations = new List<JourneyConstraintViolationResponse>();

        // 1. Evaluate domain-level invariants
        var domainValidation = journey.Validate();
        if (!domainValidation.IsValid)
        {
            invariantErrors.AddRange(domainValidation.Errors);
        }

        // 2. Day count consistency
        if (journey.Days.Count != journey.TotalDays)
        {
            violations.Add(new JourneyConstraintViolationResponse(
                null,
                null,
                "DayCountMismatch",
                $"Journey defines {journey.TotalDays} total days but contains {journey.Days.Count} itinerary days."));
        }

        // 3. Daily constraints (capacity, temporal overlaps, chronological sequence)
        foreach (var day in journey.Days)
        {
            // Capacity constraint
            if (day.Items.Count > MaxStopsPerDay)
            {
                violations.Add(new JourneyConstraintViolationResponse(
                    day.DayNumber,
                    null,
                    "DailyCapacityExceeded",
                    $"Day {day.DayNumber} contains {day.Items.Count} scheduled stops, exceeding maximum daily capacity of {MaxStopsPerDay}."));
            }

            // Evaluate time window overlaps and sequence consistency
            var timedItems = day.Items
                .Where(i => i.StartTime.HasValue && i.EndTime.HasValue)
                .OrderBy(i => i.StartTime!.Value)
                .ToList();

            for (int i = 0; i < timedItems.Count - 1; i++)
            {
                var current = timedItems[i];
                var next = timedItems[i + 1];

                if (current.EndTime!.Value > next.StartTime!.Value)
                {
                    violations.Add(new JourneyConstraintViolationResponse(
                        day.DayNumber,
                        next.Id,
                        "TimeOverlap",
                        $"Day {day.DayNumber} stop '{next.Id}' starting at {next.StartTime} overlaps with preceding stop '{current.Id}' ending at {current.EndTime}."));
                }
            }

            // Check sort order vs chronological order
            var orderedBySort = day.Items.OrderBy(i => i.SortOrder).ToList();
            for (int i = 0; i < orderedBySort.Count - 1; i++)
            {
                var cur = orderedBySort[i];
                var nxt = orderedBySort[i + 1];

                if (cur.StartTime.HasValue && nxt.StartTime.HasValue && cur.StartTime.Value > nxt.StartTime.Value)
                {
                    violations.Add(new JourneyConstraintViolationResponse(
                        day.DayNumber,
                        nxt.Id,
                        "ChronologicalSequenceMismatch",
                        $"Day {day.DayNumber} stop (order {nxt.SortOrder}) is scheduled earlier ({nxt.StartTime}) than prior stop (order {cur.SortOrder}, time {cur.StartTime})."));
                }
            }
        }

        bool isValid = invariantErrors.Count == 0 &&
                       violations.All(v => v.ConstraintType != "TimeOverlap" && v.ConstraintType != "DailyCapacityExceeded");

        return new JourneyValidationResponse(
            journey.Id,
            isValid,
            invariantErrors.AsReadOnly(),
            violations.AsReadOnly());
    }
}
