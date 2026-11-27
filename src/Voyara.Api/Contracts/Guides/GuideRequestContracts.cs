namespace Voyara.Api.Contracts.Guides
{
    /// <summary>Body of the create guide request endpoint. The tourist is taken from the access token, never from the body.</summary>
    public sealed record CreateGuideRequestRequest(
        Guid GuideProfileId,
        DateOnly Date,
        TimeOnly StartTime,
        int DurationHours,
        int PartySize,
        string PreferredLanguage,
        string? SpecialRequirements);

    /// <summary>Body of the accept endpoint: the terms the guide agrees to.</summary>
    public sealed record AcceptGuideRequestRequest(string MeetingLocation, decimal AgreedPrice, string Currency);

    /// <summary>Body of the reject endpoint.</summary>
    public sealed record RejectGuideRequestRequest(string Reason);

    /// <summary>Body of the send message endpoint.</summary>
    public sealed record SendChatMessageRequest(string Text);

    /// <summary>Body of the create review endpoint. Rating statistics are computed server-side.</summary>
    public sealed record CreateGuideReviewRequest(Guid GuideRequestId, int Rating, string Text);

}
