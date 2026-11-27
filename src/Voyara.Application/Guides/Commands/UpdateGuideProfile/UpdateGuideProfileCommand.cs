using Voyara.Application.Abstractions.Authorization;
using Voyara.Application.Abstractions.Messaging;
using Voyara.Application.DTOs.Guide;

namespace Voyara.Application.Guides.Commands.UpdateGuideProfile;

/// <summary>
/// Replaces the editable content of the authenticated user's own profile.
/// The collections describe the desired final state; the handler applies the difference.
/// </summary>
[AuthorizeRequest]
public sealed record UpdateGuideProfileCommand(
    string Bio,
    string? PhotoUrl,
    int YearsOfExperience,
    IReadOnlyList<GuideLanguageInput> Languages,
    IReadOnlyList<int> ExpertiseCategoryIds,
    IReadOnlyList<GuideAvailabilityInput> Availability,
    IReadOnlyList<GuidePricingInput> Pricing) : ICommand;
