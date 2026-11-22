namespace Voyara.Application.Guides;

/// <summary>Pure scheduling rules shared by the request use cases (all times are interpreted as the guide's local day).</summary>
internal static class GuideRequestScheduling
{
    private const int MinutesPerDay = 24 * 60;

    /// <summary>True when the guide is not on time off and one availability window fully covers the tour.</summary>
    public static bool IsWithinAvailability(GuideBookingContext guide, DateOnly date, TimeOnly start, int durationHours)
    {
        if (guide.TimeOffDates.Contains(date))
            return false;

        var startMinutes = ToMinutes(start);
        var endMinutes = startMinutes + durationHours * 60;
        if (endMinutes > MinutesPerDay)
            return false;

        return guide.Availability.Any(a =>
            a.Day == date.DayOfWeek
            && ToMinutes(a.StartTime) <= startMinutes
            && endMinutes <= ToMinutes(a.EndTime));
    }

    /// <summary>True when two same-day time ranges overlap.</summary>
    public static bool Overlaps(TimeOnly startA, int hoursA, TimeOnly startB, int hoursB)
    {
        var a1 = ToMinutes(startA);
        var a2 = a1 + hoursA * 60;
        var b1 = ToMinutes(startB);
        var b2 = b1 + hoursB * 60;
        return a1 < b2 && b1 < a2;
    }

    private static int ToMinutes(TimeOnly time) => time.Hour * 60 + time.Minute;
}
