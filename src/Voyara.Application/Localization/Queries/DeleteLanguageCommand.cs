using Voyara.Application.Abstractions.Messaging;

namespace Voyara.Application.Localization.Commands.DeleteLanguage;

public sealed record DeleteLanguageCommand(string Code) : ICommand;