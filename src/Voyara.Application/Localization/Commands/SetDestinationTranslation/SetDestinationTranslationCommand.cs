using Voyara.Application.Abstractions.Messaging;

namespace Voyara.Application.Localization.Commands.SetDestinationTranslation;

public sealed record SetDestinationTranslationCommand(
    Guid DestinationId,
    string LanguageCode,
    string Name,
    string? Description,
    string? CultureNotes) : ICommand;