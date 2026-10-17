using Voyara.Domain.Enums;
using Voyara.SharedKernel.Entities;

namespace Voyara.Domain.Entities.TripAndJourney;

/// <summary>
/// Represents a discrete single day within a journey itinerary.
/// </summary>
/// <remarks>
/// Organizes and sequences the collection of stops and activities scheduled for a specific itinerary day.
/// </remarks>
public sealed class JourneyDay : BaseEntity
{
    private readonly List<JourneyItem> _items = new();

    /// <summary>
    /// Gets the identifier of the parent journey aggregate.
    /// </summary>
    /// <value>The foreign key pointing to the owning <see cref="Journey"/>.</value>
    public Guid JourneyId { get; private set; }

    /// <summary>
    /// Gets the 1-based sequential number of this day within the journey (e.g., Day 1, Day 2).
    /// </summary>
    /// <value>A positive integer indicating day sequence.</value>
    public int DayNumber { get; private set; }

    /// <summary>
    /// Gets the optional calendar date assigned to this day.
    /// </summary>
    /// <value>The assigned <see cref="DateOnly"/>, or <see langword="null"/> if unassigned.</value>
    public DateOnly? Date { get; private set; }

    /// <summary>
    /// Gets the ordered sequence of destination stops and activities for this day.
    /// </summary>
    /// <value>A read-only collection of <see cref="JourneyItem"/> entities.</value>
    public IReadOnlyCollection<JourneyItem> Items => _items.OrderBy(i => i.SortOrder).ToList().AsReadOnly();

    private JourneyDay()
    {
    }

    private JourneyDay(Guid journeyId, int dayNumber, DateOnly? date)
    {
        JourneyId = journeyId;
        DayNumber = Math.Max(1, dayNumber);
        Date = date;
    }

    /// <summary>
    /// Factory method to create a new <see cref="JourneyDay"/> entity.
    /// </summary>
    /// <param name="journeyId">The identifier of the owning journey.</param>
    /// <param name="dayNumber">The 1-based index of this day.</param>
    /// <param name="date">Optional calendar date for the day.</param>
    /// <returns>A new <see cref="JourneyDay"/> instance.</returns>
    public static JourneyDay Create(Guid journeyId, int dayNumber, DateOnly? date = null) =>
        new(journeyId, dayNumber, date);

    /// <summary>
    /// Adds a scheduled stop item to this day.
    /// </summary>
    /// <param name="destinationId">The destination identifier.</param>
    /// <param name="cityId">Optional host city identifier.</param>
    /// <param name="startTime">Optional start time.</param>
    /// <param name="endTime">Optional end time.</param>
    /// <param name="transport">Mode of transportation to the stop.</param>
    /// <returns>The newly created and added <see cref="JourneyItem"/>.</returns>
    public JourneyItem AddItem(
        Guid destinationId,
        int? cityId = null,
        TimeSpan? startTime = null,
        TimeSpan? endTime = null,
        TransportMode transport = TransportMode.Walking)
    {
        int nextSortOrder = _items.Count > 0 ? _items.Max(i => i.SortOrder) + 1 : 1;
        var item = JourneyItem.Create(Id, destinationId, cityId, nextSortOrder, startTime, endTime, transport);
        _items.Add(item);
        return item;
    }

    /// <summary>
    /// Inserts an existing <see cref="JourneyItem"/> into this day, adjusting its sort order.
    /// </summary>
    /// <param name="item">The item to insert.</param>
    /// <param name="order">The desired sequence order index.</param>
    public void InsertItem(JourneyItem item, int order)
    {
        ArgumentNullException.ThrowIfNull(item);
        item.UpdateSortOrder(order);
        _items.Add(item);
        ReorderItems();
    }

    /// <summary>
    /// Removes a stop item from this day by identifier.
    /// </summary>
    /// <param name="itemId">The identifier of the item to remove.</param>
    /// <returns><see langword="true"/> if found and removed; otherwise, <see langword="false"/>.</returns>
    public bool RemoveItem(Guid itemId)
    {
        var item = _items.FirstOrDefault(i => i.Id == itemId);
        if (item is null)
        {
            return false;
        }

        _items.Remove(item);
        ReorderItems();
        return true;
    }

    /// <summary>
    /// Re-indexes sort orders of contained items to be contiguous starting from 1.
    /// </summary>
    public void ReorderItems()
    {
        var sorted = _items.OrderBy(i => i.SortOrder).ToList();
        for (int i = 0; i < sorted.Count; i++)
        {
            sorted[i].UpdateSortOrder(i + 1);
        }
    }

    /// <summary>
    /// Updates the calendar date assigned to this day.
    /// </summary>
    /// <param name="date">The new calendar date.</param>
    public void SetDate(DateOnly? date)
    {
        Date = date;
    }
}


