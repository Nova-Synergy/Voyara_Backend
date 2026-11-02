using Voyara.Application.Abstractions.Authorization;
using Voyara.Application.Abstractions.Messaging;

namespace Voyara.Application.Community.Commands.DeleteReview;

/// <summary>
/// Command to delete an existing review for a destination.</summary>
/// <remarks>
/// <para>
/// Requires authorization for Tourist role to execute.</para>
/// <param name="ReviewId">The unique identifier of the review to delete.</param>
/// </remarks>
[AuthorizeRequest(Roles.Tourist)]
public sealed record DeleteReviewCommand(Guid ReviewId) : ICommand;