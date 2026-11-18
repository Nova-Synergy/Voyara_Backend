using Voyara.Application.Abstractions.Authorization;
using Voyara.Application.Abstractions.Messaging;

namespace Voyara.Application.Guides.Commands.SubmitGuideApplication;

/// <summary>
/// Confirms that the authenticated user's pending application is complete and ready for review.
/// Creating the profile already put it in Pending (ST-04), so this command changes no state.
/// </summary>
[AuthorizeRequest]
public sealed record SubmitGuideApplicationCommand : ICommand;
