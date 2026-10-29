using Voyara.Domain.Entities.Geography;

namespace Voyara.Domain.Repositories.Geography
{

    /// <summary>
    /// Read-side lookups for <see cref="City"/>. Cities are created and changed only through
    /// <see cref="Country"/>, so this contract has no add/update operations.
    /// </summary>
    public interface ICityRepository
    {
    }
}