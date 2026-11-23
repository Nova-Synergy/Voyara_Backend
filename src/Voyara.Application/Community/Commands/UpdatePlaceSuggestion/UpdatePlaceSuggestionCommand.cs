using Voyara.Application.Abstractions.Authorization;
using Voyara.Application.Abstractions.Messaging;

namespace Voyara.Application.Community.Commands.UpdatePlaceSuggestion;

/// <summary>
/// Command to update an existing pending place suggestion owned by the currently authenticated tourist.
/// </summary>
/// <param name="SuggestionId">The unique identifier of the place suggestion to update.</param>
/// <param name="Name">The updated proposed name of the place.</param>
/// <param name="Description">The updated detailed description of the place.</param>
/// <param name="Latitude">The updated geographic latitude coordinate of the place.</param>
/// <param name="Longitude">The updated geographic longitude coordinate of the place.</param>
/// <param name="Reason">The updated rationale for suggesting the place.</param>
[AuthorizeRequest(Roles.Tourist)]
public sealed record UpdatePlaceSuggestionCommand(
    Guid SuggestionId,
    string Name,
    string Description,
    double Latitude,
    double Longitude,
    string Reason) : ICommand;