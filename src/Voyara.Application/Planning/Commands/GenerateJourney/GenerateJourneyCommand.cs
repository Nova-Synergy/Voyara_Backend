using Voyara.Application.Abstractions.Messaging;

namespace Voyara.Application.Planning.Commands.GenerateJourney;

/// <summary>
/// Command to generate a tailored journey itinerary using destination catalog data and AI assistant synthesis.
/// </summary>
/// <param name="Title">Optional user-provided title; if omitted, synthesized automatically.</param>
/// <param name="DurationDays">Planned itinerary duration in days (minimum 1).</param>
/// <param name="Interests">Tourist interest keywords used for destination matching.</param>
/// <param name="TravelStyle">Travel tempo preference (e.g. "Relaxed", "Adventure", "Cultural").</param>
/// <param name="TransportPreference">Preferred transport mode (e.g. "Walking", "PublicTransport", "Driving").</param>
/// <param name="Country">Optional destination country or region filter.</param>
/// <param name="BudgetAmount">Optional budget ceiling.</param>
/// <param name="BudgetCurrency">Optional currency code for budget (e.g. "USD").</param>
public sealed record GenerateJourneyCommand(
    string? Title,
    int DurationDays,
    IReadOnlyList<string> Interests,
    string TravelStyle,
    string TransportPreference,
    string? Country = null,
    decimal? BudgetAmount = null,
    string? BudgetCurrency = null) : ICommand<Guid>;
