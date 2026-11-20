using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Voyara.Domain.Entities.Guides;

namespace Voyara.Domain.Repositories.Guides
{
    /// <summary>
    /// Defines repository operations for guide documents.
    /// </summary>
    public interface IGuideDocumentRepository
    {
        /// <summary>Adds a document record.</summary>
        void Add(GuideDocument document);

        /// <summary>Removes (soft-deletes) a document record.</summary>
        void Remove(GuideDocument document);

        /// <summary>Gets a tracked document by identifier.</summary>
        Task<GuideDocument?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

        /// <summary>Counts the documents of a profile.</summary>
        Task<int> CountByProfileIdAsync(Guid guideProfileId, CancellationToken cancellationToken = default);
    }
}
