using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Voyara.Domain.Entities.Guides;
using Voyara.Domain.Repositories.Guides;

namespace Voyara.Infrastructure.Persistence.Repositories.Guides
{
    public sealed class GuideDocumentRepository : IGuideDocumentRepository
    {
        private readonly VoyaraDbContext _context;

        public GuideDocumentRepository(VoyaraDbContext context)
        {
            _context = context;
        }

        public void Add(GuideDocument document) => _context.Set<GuideDocument>().Add(document);

        public void Remove(GuideDocument document) => _context.Set<GuideDocument>().Remove(document);

        public Task<GuideDocument?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
            _context.Set<GuideDocument>().FirstOrDefaultAsync(d => d.Id == id, cancellationToken);

        public Task<int> CountByProfileIdAsync(Guid guideProfileId, CancellationToken cancellationToken = default) =>
            _context.Set<GuideDocument>().CountAsync(d => d.GuideProfileId == guideProfileId, cancellationToken);
    }
}
