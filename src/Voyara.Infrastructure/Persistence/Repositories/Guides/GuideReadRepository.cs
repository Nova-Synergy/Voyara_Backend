using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Voyara.Application.DTOs;
using Voyara.Application.DTOs.Guide;
using Voyara.Application.Guides;
using Voyara.Domain.Entities.Guides;
using Voyara.Domain.Enums;

namespace Voyara.Infrastructure.Persistence.Repositories.Guides
{
    public sealed class GuideReadRepository : IGuideReadRepository
    {
        private readonly VoyaraDbContext _context;

        public GuideReadRepository(VoyaraDbContext context) => _context = context;

        public async Task<PagedResult<GuideListItemDto>> SearchApprovedAsync(
            GuideSearchCriteria criteria, int page, int pageSize, CancellationToken cancellationToken = default)
        {
            var query = _context.Set<GuideProfile>()
                .AsNoTracking()
                .Where(p => p.Status == GuideStatus.Approved && !p.IsDeleted);

            if (criteria.CityId is { } cityId)
                query = query.Where(p => p.CityId == cityId);

            if (criteria.LanguageCode is { } languageCode)
                query = query.Where(p => p.Languages.Any(l => !l.IsDeleted && l.LanguageCode == languageCode));

            if (criteria.ExpertiseCategoryId is { } categoryId)
                query = query.Where(p => p.Expertise.Any(e => !e.IsDeleted && e.CategoryId == categoryId));

            if (criteria.MinRating is { } minRating)
                query = query.Where(p => p.AverageRating >= minRating);

            if (criteria.MinPrice.HasValue || criteria.MaxPrice.HasValue)
            {
                var unit = criteria.PricingUnit ?? PricingUnit.PerHour;
                var currency = criteria.Currency ?? "USD";
                var minPrice = criteria.MinPrice;
                var maxPrice = criteria.MaxPrice;

                query = query.Where(p => p.Pricing.Any(price =>
                    !price.IsDeleted
                    && price.Unit == unit
                    && price.Currency == currency
                    && (minPrice == null || price.Amount >= minPrice)
                    && (maxPrice == null || price.Amount <= maxPrice)));
            }

            var total = await query.CountAsync(cancellationToken);

            var items = await query
                .OrderByDescending(p => p.AverageRating)
                .ThenByDescending(p => p.ReviewCount)
                .ThenBy(p => p.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new GuideListItemDto(
                    p.Id,
                    p.User!.FullName,
                    p.PhotoUrl,
                    p.YearsOfExperience,
                    p.AverageRating,
                    p.ReviewCount,
                    p.CityId,
                    p.Languages.Where(l => !l.IsDeleted).Select(l => l.LanguageCode).ToList(),
                    p.Pricing.Where(x => !x.IsDeleted)
                        .Select(x => new GuidePricingDto(x.Unit, x.Amount, x.Currency)).ToList()))
                .ToListAsync(cancellationToken);

            return new PagedResult<GuideListItemDto>(items, total, page, pageSize);
        }

        public async Task<PagedResult<GuideApplicationListItemDto>> ListApplicationsAsync(
            GuideStatus? status, int page, int pageSize, CancellationToken cancellationToken = default)
        {
            var query = _context.Set<GuideProfile>().AsNoTracking().Where(p => !p.IsDeleted);

            if (status is { } filter)
                query = query.Where(p => p.Status == filter);

            var total = await query.CountAsync(cancellationToken);

            var items = await query
                .OrderBy(p => p.CreatedAt)
                .ThenBy(p => p.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new GuideApplicationListItemDto(
                    p.Id, p.UserId, p.User!.FullName, p.CityId, p.Status, p.CreatedAt))
                .ToListAsync(cancellationToken);

            return new PagedResult<GuideApplicationListItemDto>(items, total, page, pageSize);
        }

        public Task<GuideDetailsReadModel?> GetDetailsAsync(Guid profileId, CancellationToken cancellationToken = default) =>
            ProjectDetails(_context.Set<GuideProfile>().AsNoTracking()
                .Where(p => p.Id == profileId && !p.IsDeleted))
                .FirstOrDefaultAsync(cancellationToken);

        public Task<GuideDetailsReadModel?> GetDetailsByUserIdAsync(Guid userId, CancellationToken cancellationToken = default) =>
            ProjectDetails(_context.Set<GuideProfile>().AsNoTracking()
                .Where(p => p.UserId == userId && !p.IsDeleted))
                .FirstOrDefaultAsync(cancellationToken);

        private static IQueryable<GuideDetailsReadModel> ProjectDetails(IQueryable<GuideProfile> query) =>
            query.Select(p => new GuideDetailsReadModel(
                new GuideProfileDetailsDto(
                    p.Id,
                    p.User!.FullName,
                    p.PhotoUrl,
                    p.Bio,
                    p.YearsOfExperience,
                    p.AverageRating,
                    p.ReviewCount,
                    p.CityId,
                    p.Status,
                    p.Languages.Where(l => !l.IsDeleted)
                        .Select(l => new GuideLanguageDto(l.LanguageCode, l.Proficiency)).ToList(),
                    p.Expertise.Where(e => !e.IsDeleted).Select(e => e.CategoryId).ToList(),
                    p.Availability.Where(a => !a.IsDeleted)
                        .Select(a => new GuideAvailabilityDto(a.Day, a.StartTime, a.EndTime)).ToList(),
                    p.Pricing.Where(x => !x.IsDeleted)
                        .Select(x => new GuidePricingDto(x.Unit, x.Amount, x.Currency)).ToList(),
                    null),
                p.UserId,
                p.RejectionReason));
    }
}
