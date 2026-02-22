using BusinessObject.Entities;
using DataAccessObject;
using Microsoft.EntityFrameworkCore;
using Repositories.Abstractions;

namespace Repositories
{
    public class NewsArticleRepository : Repository<NewsArticle, string>, INewsArticleRepository
    {
        public NewsArticleRepository(FunewsManagementContext context) : base(context)
        {
        }

        public async Task<List<NewsArticle>> GetAllAsync(string? keyword = null, short? categoryId = null, bool? newsStatus = null, short? createdById = null, DateTime? startDate = null, DateTime? endDate = null, int? pageIndex = null, int? pageSize = null)
        {
            var query = _context.NewsArticles
                .Include(x => x.Category)
                .Include(x => x.Tags)
                .Include(x => x.CreatedBy)
                .AsQueryable();

            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(x => x.NewsTitle.Contains(keyword) || x.Headline.Contains(keyword));
            }

            if (categoryId.HasValue)
            {
                query = query.Where(x => x.CategoryId == categoryId.Value);
            }

            if (newsStatus.HasValue)
            {
                query = query.Where(x => x.NewsStatus == newsStatus.Value);
            }

            if (createdById.HasValue)
            {
                query = query.Where(x => x.CreatedById == createdById.Value);
            }

            if (startDate.HasValue)
            {
                query = query.Where(x => x.CreatedDate >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                query = query.Where(x => x.CreatedDate <= endDate.Value);
            }

            query = query.OrderByDescending(x => x.CreatedDate);

            if (pageIndex.HasValue && pageSize.HasValue)
            {
                query = query.Skip((pageIndex.Value - 1) * pageSize.Value).Take(pageSize.Value);
            }

            return await query.ToListAsync();
        }

        public async Task<int> CountAsync(string? keyword = null, short? categoryId = null, bool? newsStatus = null, short? createdById = null, DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = _context.NewsArticles.AsQueryable();

            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(x => x.NewsTitle.Contains(keyword) || x.Headline.Contains(keyword));
            }

            if (categoryId.HasValue)
            {
                query = query.Where(x => x.CategoryId == categoryId.Value);
            }

            if (newsStatus.HasValue)
            {
                query = query.Where(x => x.NewsStatus == newsStatus.Value);
            }

            if (createdById.HasValue)
            {
                query = query.Where(x => x.CreatedById == createdById.Value);
            }

            if (startDate.HasValue)
            {
                query = query.Where(x => x.CreatedDate >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                query = query.Where(x => x.CreatedDate <= endDate.Value);
            }

            return await query.CountAsync();
        }

        public async Task<List<NewsArticle>> GetRelatedNewsAsync(string currentNewsId, List<int> tagIds)
        {
            var query = _context.NewsArticles
                .Include(x => x.Tags)
                .Where(x => !x.Id.Equals(currentNewsId));

            if (tagIds != null && tagIds.Any())
            {
                query = query.Where(x =>
                    x.Tags.Any(t => tagIds.Contains(t.Id)));
            }

            return await query
                .OrderByDescending(x => x.CreatedDate)
                .Take(5)
                .ToListAsync();
        }
    }
}
