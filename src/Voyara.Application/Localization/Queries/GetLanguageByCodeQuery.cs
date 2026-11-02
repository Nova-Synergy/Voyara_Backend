using Voyara.Application.Abstractions.Messaging;
using Voyara.Domain.Entities.Localization;

namespace Voyara.Application.Localization.Queries.GetLanguageByCode;

public sealed record GetLanguageByCodeQuery(string Code)
    : IQuery<Language>;