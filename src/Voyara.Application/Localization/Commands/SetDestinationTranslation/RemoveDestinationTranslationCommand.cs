using Voyara.Application.Abstractions.Messaging;

namespace Voyara.Application.Localization.Commands.RemoveDestinationTranslation;

public sealed record RemoveDestinationTranslationCommand(
    Guid DestinationId,
    string LanguageCode) : ICommand;