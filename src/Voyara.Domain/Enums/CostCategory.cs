namespace Voyara.Domain.Enums;

/// <summary>
/// Categorizes financial expenses and budgeting line items across travel plans.
/// </summary>
/// <remarks>
/// Used by cost rules, cost estimation breakdowns, and expense tracking to aggregate spend
/// across standard tourist cost categories.
/// </remarks>
public enum CostCategory
{
    /// <summary>
    /// Transit expenses, including airfare, train tickets, taxis, and public transport fares.
    /// </summary>
    Transportation,

    /// <summary>
    /// Lodging costs, including hotels, resorts, rentals, or hostels.
    /// </summary>
    Accommodation,

    /// <summary>
    /// Admission tickets to attractions, museums, heritage sites, and events.
    /// </summary>
    Tickets,

    /// <summary>
    /// Dining, groceries, beverages, and culinary experiences.
    /// </summary>
    Food,

    /// <summary>
    /// Guided tours, excursions, sports, entertainment, and workshops.
    /// </summary>
    Activities,

    /// <summary>
    /// Professional local guide or translator service fees.
    /// </summary>
    TourGuide,

    /// <summary>
    /// Incidental expenses, souvenirs, visa fees, gratuities, or emergency funds.
    /// </summary>
    Miscellaneous
}

