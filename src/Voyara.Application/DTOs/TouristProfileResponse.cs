namespace Voyara.Application.DTOs;

/// <summary>Read model of the signed-in tourist's profile and preferences.</summary>
public sealed record TouristProfileResponse(
    Guid UserId,
    string FullName,
    string Email,
    string NationalityCode,
    string PreferredLanguage,
    string Budget,
    string? PhotoUrl,
    IReadOnlyList<InterestResponse> Interests);
