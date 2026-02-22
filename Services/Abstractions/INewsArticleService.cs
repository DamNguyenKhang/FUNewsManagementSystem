using BusinessObject.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services.Abstractions
{
    public interface INewsArticleService
    {
        Task<List<NewsArticle>> GetAllNews(string? search = null, short? categoryId = null, bool? status = null, short? createdById = null, DateTime? startDate = null, DateTime? endDate = null, int? pageIndex = null, int? pageSize = null);
        Task<int> GetNewsCount(string? search = null, short? categoryId = null, bool? status = null, short? createdById = null, DateTime? startDate = null, DateTime? endDate = null);
        Task<NewsArticle?> GetNewsById(string id);
        Task<List<NewsArticle>> GetRelatedNews(string currentArticleId, List<int> tagIds);
        Task AddNewsArticle(NewsArticle article, List<int> tagIds, Microsoft.AspNetCore.Http.IFormFile? imageFile = null);
        Task UpdateNewsArticle(NewsArticle article, List<int> tagIds, Microsoft.AspNetCore.Http.IFormFile? imageFile = null);
        Task DeleteNewsArticle(string id);
    }
}
