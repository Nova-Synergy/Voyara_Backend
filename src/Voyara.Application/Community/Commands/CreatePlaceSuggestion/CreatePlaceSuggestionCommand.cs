using Voyara.Application.Abstractions.Authorization;
using Voyara.Application.Abstractions.Messaging;

namespace Voyara.Application.Community.Commands.CreatePlaceSuggestion;

/// <summary>
/// Command to create a new place suggestion on behalf of the currently authenticated tourist.
/// </summary>
/// <param name="Name">The proposed name of the place.</param>
/// <param name="Description">A detailed description of the suggested place.</param>
/// <param name="Latitude">The geographic latitude coordinate of the place.</param>
/// <param name="Longitude">The geographic longitude coordinate of the place.</param>
/// <param name="Reason">The tourist's rationale for recommending this place.</param>
[AuthorizeRequest(Roles.Tourist)]
public sealed record CreatePlaceSuggestionCommand(
    string Name,
    string Description,
    double Latitude,
    double Longitude,
    string Reason) : ICommand<Guid>;