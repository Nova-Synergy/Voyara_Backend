using Voyara.Application.Abstractions.Authorization;
using Voyara.Application.Abstractions.Messaging;
using Voyara.Application.DTOs.Guide;

namespace Voyara.Application.Guides.Commands.CreateGuideProfile;

/// <summary>
/// Creates the profile of the authenticated user. The owner is taken from the current user,
/// never from the request. The new profile starts as Pending.
/// </summary>
[AuthorizeRequest]
public sealed record CreateGuideProfileCommand(
    Guid CityId,
    string Bio,
    string? PhotoUrl,
    int YearsOfExperience,
    IReadOnlyList<GuideLanguageInput> Languages,
    IReadOnlyList<int> ExpertiseCategoryIds,
    IReadOnlyList<GuideAvailabilityInput> Availability,
    IReadOnlyList<GuidePricingInput> Pricing) : ICommand<Guid>;
