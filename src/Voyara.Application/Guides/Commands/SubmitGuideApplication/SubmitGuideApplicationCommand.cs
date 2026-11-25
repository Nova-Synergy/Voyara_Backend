using Voyara.Application.Abstractions.Authorization;
using Voyara.Application.Abstractions.Messaging;

namespace Voyara.Application.Guides.Commands.SubmitGuideApplication;

/// <summary>
/// Confirms that the authenticated user's application is complete and ready for review.
/// A Pending application is only validated (creating the profile already submitted it, ST-04).
/// Resubmitting a Rejected application needs a domain transition that does not exist yet.
/// </summary>
[AuthorizeRequest]
public sealed record SubmitGuideApplicationCommand : ICommand;
