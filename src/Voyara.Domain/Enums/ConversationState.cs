namespace Voyara.Domain.Enums;

/// <summary>
/// Represents the conversational workflow state of an AI assistant interaction session.
/// </summary>
public enum ConversationState
{
    /// <summary>
    /// The conversation is active and continuing to engage with the user.
    /// </summary>
    Active,

    /// <summary>
    /// The assistant has collected and validated the user's travel requirements.
    /// </summary>
    RequirementsCollected,

    /// <summary>
    /// A draft journey itinerary has been generated based on conversation requirements.
    /// </summary>
    JourneyGenerated,

    /// <summary>
    /// The conversation has successfully fulfilled its objective and ended.
    /// </summary>
    Completed,

    /// <summary>
    /// The conversation has been archived and is retained for historical reference only.
    /// </summary>
    Archived
}

