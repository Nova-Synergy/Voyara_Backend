using Voyara.Application.Abstractions.Messaging;
using Voyara.Domain.Entities.Localization;

namespace Voyara.Application.Localization.Queries.GetDestinationTranslations;

public sealed record GetDestinationTranslationsQuery(Guid DestinationId)
    : IQuery<IReadOnlyCollection<DestinationTranslation>>;