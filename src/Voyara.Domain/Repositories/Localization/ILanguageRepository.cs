using Voyara.Domain.Entities.Localization;

namespace Voyara.Domain.Repositories.Localization;

public interface ILanguageRepository
{
    Task<Language?> GetByCodeAsync(
        string code,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Language>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<bool> ExistsByCodeAsync(
        string code,
        CancellationToken cancellationToken = default);

    void Add(Language language);

    void Remove(Language language);
}