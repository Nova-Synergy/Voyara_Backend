namespace Voyara.Application.Guides;

/// <summary>Pure scheduling rules shared by the request use cases (all times are interpreted as the guide's local day).</summary>
public static class GuideRequestScheduling
{
    private const int MinutesPerDay = 24 * 60;

    /// <summary>Resolves an IANA/system time zone id; falls back to UTC when it is missing or unknown.</summary>
    public static TimeZoneInfo ResolveTimeZone(string? timeZoneId)
    {
        if (string.IsNullOrWhiteSpace(timeZoneId))
            return TimeZoneInfo.Utc;

        try
        {
            return TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
        }
        catch (TimeZoneNotFoundException)
        {
            return TimeZoneInfo.Utc;
        }
        catch (InvalidTimeZoneException)
        {
            return TimeZoneInfo.Utc;
        }
    }

    /// <summary>The wall-clock time currently shown on a clock in the guide's time zone.</summary>
    public static DateTime GuideLocalNow(DateTimeOffset utcNow, string? timeZoneId) =>
        TimeZoneInfo.ConvertTime(utcNow, ResolveTimeZone(timeZoneId)).DateTime;

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
