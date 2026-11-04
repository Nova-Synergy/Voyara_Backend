using Voyara.Application.Abstractions.Authorization;
using Voyara.Application.Abstractions.Messaging;

namespace Voyara.Application.Identity.Commands.TourGuides.DeleteTourGuide;

/// <summary>Soft-deletes the guide profile (user account stays). Owner or admin.</summary>
[AuthorizeRequest]
public sealed record DeleteTourGuideCommand(Guid Id) : ICommand;
