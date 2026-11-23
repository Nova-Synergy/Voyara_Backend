using Voyara.Application.Abstractions.Messaging;

namespace Voyara.Application.Localization.Commands.UpdateLanguage;

public sealed record UpdateLanguageCommand(
    string Code,
    string Name,
    bool IsRtl) : ICommand;