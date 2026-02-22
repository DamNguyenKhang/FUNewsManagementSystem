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

        public async Task<List<NewsArticle>> GetAllAsync(string? keyword = null, short? categoryId = null, bool? newsStatus = null, short? createdById = null, DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = _context.NewsArticles
                .Include(x => x.Category)
                .Include(x => x.Tags)
                .Include(x => x.CreatedBy)
                .AsQueryable();

            // Filter theo Date Range
            if (startDate.HasValue)
            {
                query = query.Where(x => x.CreatedDate >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                query = query.Where(x => x.CreatedDate <= endDate.Value);
            }

            // Filter theo CreatedBy
            if (createdById.HasValue)
            {
                query = query.Where(x => x.CreatedById == createdById.Value);
            }

            // Filter theo Category
            if (categoryId.HasValue)
            {
                query = query.Where(x => x.CategoryId == categoryId.Value);
            }

            // Filter theo Status
            if (newsStatus.HasValue)
            {
                query = query.Where(x => x.NewsStatus == newsStatus.Value);
            }

            // Search theo keyword
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                keyword = keyword.Trim();

                query = query.Where(x =>
                    EF.Functions.Like(x.Headline, $"%{keyword}%") ||
                    EF.Functions.Like(x.NewsContent, $"%{keyword}%") ||
                    EF.Functions.Like(x.Category.CategoryName, $"%{keyword}%") ||
                    x.Tags.Any(t => EF.Functions.Like(t.TagName, $"%{keyword}%"))
                );
            }

            return await query
                .OrderByDescending(x => x.CreatedDate)
                .ToListAsync();
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
