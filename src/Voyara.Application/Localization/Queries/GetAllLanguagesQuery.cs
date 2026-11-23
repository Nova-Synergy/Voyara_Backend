using Voyara.Application.Abstractions.Messaging;
using Voyara.Domain.Entities.Localization;

namespace Voyara.Application.Localization.Queries.GetAllLanguages;

public sealed record GetAllLanguagesQuery
    : IQuery<IReadOnlyList<Language>>;