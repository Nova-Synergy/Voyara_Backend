using Voyara.Application.DTOs;
using Voyara.Domain.Entities.Identity;

namespace Voyara.Application.Mappings;

/// <summary>Maps the tourist profile aggregate to its read models.</summary>
internal static class TouristProfileMappings
{
    /// <summary>Maps a profile (loaded with its user and interests) to <see cref="TouristProfileResponse"/>.</summary>
    public static TouristProfileResponse ToResponse(this TouristProfile profile)
        => new(
            profile.UserId,
            profile.User.FullName,
            profile.User.Email,
            profile.NationalityCode,
            profile.PreferredLanguage,
            profile.Budget.ToString(),
            profile.PhotoUrl,
            profile.Interests
                .OrderBy(interest => interest.Code)
                .Select(interest => new InterestResponse(interest.Id, interest.Code))
                .ToList());

    /// <summary>Maps an interest to <see cref="InterestResponse"/>.</summary>
    public static InterestResponse ToResponse(this Interest interest) => new(interest.Id, interest.Code);
}
