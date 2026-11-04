using Voyara.Application.Abstractions.Messaging;

namespace Voyara.Application.Localization.Commands.CreateLanguage;

public sealed record CreateLanguageCommand(
    string Code,
    string Name,
    bool IsRtl) : ICommand;