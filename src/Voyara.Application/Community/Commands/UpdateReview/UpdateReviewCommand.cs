using Voyara.Application.Abstractions.Authorization;
using Voyara.Application.Abstractions.Messaging;

namespace Voyara.Application.Community.Commands.UpdateReview;

/// <summary>
/// Command to update an existing review for a destination.</summary>
/// <remarks>
/// <para>
/// Requires authorization for Tourist role to execute.</para>
/// <param name="ReviewId">The unique identifier of the review to update.</param>
/// <param name="Rating">The new rating given, from one to five stars.</param>
/// <param name="Text">The new written review content.</param>
/// <param name="VisitDate">The new date the user visited the destination.</param>
/// </remarks>
[AuthorizeRequest(Roles.Tourist)]
public sealed record UpdateReviewCommand(
    Guid ReviewId,
    int Rating,
    string Text,
    DateOnly VisitDate) : ICommand;