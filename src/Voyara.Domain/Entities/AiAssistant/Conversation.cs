using Voyara.Domain.Entities.Identity;
using Voyara.Domain.Enums;
using Voyara.Domain.ValueObjects;
using Voyara.SharedKernel.Entities;

namespace Voyara.Domain.Entities.AiAssistant;

/// <summary>
/// Aggregate root representing an interactive AI planning conversation session between a tourist and the travel assistant.
/// </summary>
/// <remarks>
/// Encapsulates multi-turn message history, captured trip preferences (<see cref="TripRequirements"/>),
/// and the resulting itinerary reference when journey generation is requested.
/// </remarks>
public sealed class Conversation : AggregateRoot
{
    private readonly List<Message> _messages = new();

    /// <summary>
    /// Gets the identifier of the user who owns this conversation session.
    /// </summary>
    /// <value>The foreign key pointing to the owning <see cref="User"/>.</value>
    public Guid UserId { get; private set; }

    /// <summary>
    /// Gets the navigation reference to the user.
    /// </summary>
    /// <value>The owning <see cref="User"/> entity.</value>
    public User? User { get; private set; }

    /// <summary>
    /// Gets the ISO language code used for conversational dialog (e.g., "en", "ar").
    /// </summary>
    /// <value>The language code string.</value>
    public string Language { get; private set; } = "en";

    /// <summary>
    /// Gets the current state of the conversation workflow.
    /// </summary>
    /// <value>A <see cref="ConversationState"/> representing the progress of the session.</value>
    public ConversationState State { get; private set; } = ConversationState.Active;

    /// <summary>
    /// Gets the UTC timestamp when the conversation was initiated.
    /// </summary>
    /// <value>The session start date and time.</value>
    public DateTime StartedAt { get; private set; }

    /// <summary>
    /// Gets the identifier of the journey itinerary generated from this conversation, if completed.
    /// </summary>
    /// <value>The optional foreign key pointing to the created <c>Journey</c>.</value>
    public Guid? JourneyId { get; private set; }

    /// <summary>
    /// Gets the structured travel requirements and preferences extracted during the conversation.
    /// </summary>
    /// <value>The <see cref="TripRequirements"/> value object, or <see langword="null"/> if not yet established.</value>
    public TripRequirements? Requirements { get; private set; }

    /// <summary>
    /// Gets the ordered sequence of messages exchanged in this conversation.
    /// </summary>
    /// <value>An immutable view of the conversation message history.</value>
    public IReadOnlyCollection<Message> Messages => _messages.AsReadOnly();

    private Conversation()
    {
    }

    private Conversation(Guid userId, string language)
    {
        UserId = userId;
        Language = string.IsNullOrWhiteSpace(language) ? "en" : language.Trim();
        State = ConversationState.Active;
        StartedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Creates a new conversation session for a user.
    /// </summary>
    /// <param name="userId">The identifier of the user starting the chat.</param>
    /// <param name="language">The preferred interaction language code (defaults to "en").</param>
    /// <returns>A new <see cref="Conversation"/> aggregate instance.</returns>
    public static Conversation Create(Guid userId, string language = "en") =>
        new(userId, language);

    /// <summary>
    /// Appends a new message to the conversation history.
    /// </summary>
    /// <param name="role">The role of the message author (<see cref="MessageRole"/>).</param>
    /// <param name="content">The textual message content.</param>
    /// <param name="tokensUsed">The token count consumed by this message exchange.</param>
    /// <returns>The created and added <see cref="Message"/> instance.</returns>
    public Message AddMessage(MessageRole role, string content, int tokensUsed = 0)
    {
        var message = Message.Create(Id, role, content, tokensUsed);
        _messages.Add(message);
        return message;
    }

    /// <summary>
    /// Applies structured travel requirements extracted by the assistant from conversational context.
    /// </summary>
    /// <param name="requirements">The extracted <see cref="TripRequirements"/> value object.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="requirements"/> is <see langword="null"/>.</exception>
    public void ApplyRequirements(TripRequirements requirements)
    {
        ArgumentNullException.ThrowIfNull(requirements);

        Requirements = requirements;

        if (requirements.IsComplete() && State == ConversationState.Active)
        {
            State = ConversationState.RequirementsCollected;
        }
    }

    /// <summary>
    /// Links the identifier of a journey itinerary generated as an outcome of this planning conversation.
    /// </summary>
    /// <param name="journeyId">The identifier of the produced journey.</param>
    public void AttachJourney(Guid journeyId)
    {
        JourneyId = journeyId;
        State = ConversationState.JourneyGenerated;
    }

    /// <summary>
    /// Completes the conversation session, indicating no further turns are expected.
    /// </summary>
    public void Complete()
    {
        if (State != ConversationState.Archived)
        {
            State = ConversationState.Completed;
        }
    }

    /// <summary>
    /// Archives the conversation session for historical reference.
    /// </summary>
    public void Archive()
    {
        State = ConversationState.Archived;
    }
}

