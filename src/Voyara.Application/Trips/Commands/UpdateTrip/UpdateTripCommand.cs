using Voyara.Application.Abstractions.Messaging;
using Voyara.Domain.Enums;

namespace Voyara.Application.Trips.Commands.UpdateTrip;

/// <summary>
/// Command to update the metadata, dates, visibility, and status of an existing trip.
/// </summary>
/// <param name="TripId">The unique identifier of the trip to modify.</param>
/// <param name="Title">Revised title string.</param>
/// <param name="StartDate">Revised departure date.</param>
/// <param name="EndDate">Revised return date.</param>
/// <param name="Visibility">Revised access visibility setting.</param>
/// <param name="Status">Optional new lifecycle status.</param>
public sealed record UpdateTripCommand(
    Guid TripId,
    string Title,
    DateOnly StartDate,
    DateOnly EndDate,
    Visibility Visibility,
    TripStatus? Status = null) : ICommand;
