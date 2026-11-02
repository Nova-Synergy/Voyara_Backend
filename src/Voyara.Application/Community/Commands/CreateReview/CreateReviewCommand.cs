using Voyara.Application.Abstractions.Authorization;
using Voyara.Application.Abstractions.Messaging;

namespace Voyara.Application.Community.Commands.CreateReview;

/// <summary>
/// Command to create a new review for a destination.</summary>
/// <remarks>
/// <para>
/// Requires authorization for Tourist role to execute.</para>
/// <param name="DestinationId">The unique identifier of the destination to review.</param>
/// <param name="Rating">The rating given, from one to five stars.</param>
/// <param name="Text">The written review content.</param>
/// <param name="VisitDate">The date the user visited the destination.</param>
/// </remarks>
[AuthorizeRequest(Roles.Tourist)]
public sealed record CreateReviewCommand(
    Guid DestinationId,
    int Rating,
    string Text,
    DateOnly VisitDate) : ICommand<Guid>;