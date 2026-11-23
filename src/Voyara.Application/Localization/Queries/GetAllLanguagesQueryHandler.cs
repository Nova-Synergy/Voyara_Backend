using MediatR;
using Voyara.Domain.Entities.Localization;
using Voyara.Domain.Repositories.Localization;
using Voyara.SharedKernel.Results;

namespace Voyara.Application.Localization.Queries.GetAllLanguages;

public sealed class GetAllLanguagesQueryHandler
    : IRequestHandler<GetAllLanguagesQuery, Result<IReadOnlyList<Language>>>
{
    private readonly ILanguageRepository _repository;

    public GetAllLanguagesQueryHandler(ILanguageRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<IReadOnlyList<Language>>> Handle(
        GetAllLanguagesQuery request,
        CancellationToken cancellationToken)
    {
        var languages = await _repository.GetAllAsync(cancellationToken);

        return languages;
    }
}