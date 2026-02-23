using BusinessObject.Entities;

namespace Repositories.Abstractions
{
    public interface INewsArticleRepository : IRepository<NewsArticle, string>
    {
        Task<List<NewsArticle>> GetAllAsync(string? keyword = null, short? categoryId = null, bool? newsStatus = null, short? createdById = null, DateTime? startDate = null, DateTime? endDate = null, int? tagId = null, int? pageIndex = null, int? pageSize = null);
        Task<int> CountAsync(string? keyword = null, short? categoryId = null, bool? newsStatus = null, short? createdById = null, DateTime? startDate = null, DateTime? endDate = null, int? tagId = null);
        Task<List<NewsArticle>> GetRelatedNewsAsync(string currentNewsId, List<int> tagIds);
    }
}
