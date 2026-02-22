using Repositories.Abstractions;
using Services.Abstractions;
using Microsoft.EntityFrameworkCore;
using BusinessObject.Entities;
using Microsoft.AspNetCore.Http;

namespace Services
{
    public class NewsArticleService : INewsArticleService
    {
        private readonly INewsArticleRepository _newsArticleRepository;
        private readonly ITagRepository _tagRepository;
        private readonly IFileStorageService _fileStorageService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly AutoMapper.IMapper _mapper;

        public NewsArticleService(
            INewsArticleRepository newsArticleRepository, 
            ITagRepository tagRepository,
            IUnitOfWork unitOfWork,
            AutoMapper.IMapper mapper,
            IFileStorageService fileStorageService)
        {
            _newsArticleRepository = newsArticleRepository;
            _tagRepository = tagRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _fileStorageService = fileStorageService;
        }

        public async Task<List<NewsArticle>> GetAllNews(string? search = null, short? categoryId = null, bool? status = null, short? createdById = null, DateTime? startDate = null, DateTime? endDate = null, int? pageIndex = null, int? pageSize = null)
        {
            return await _newsArticleRepository.GetAllAsync(keyword: search, categoryId: categoryId, newsStatus: status, createdById: createdById, startDate: startDate, endDate: endDate, pageIndex: pageIndex, pageSize: pageSize);
        }

        public async Task<int> GetNewsCount(string? search = null, short? categoryId = null, bool? status = null, short? createdById = null, DateTime? startDate = null, DateTime? endDate = null)
        {
            return await _newsArticleRepository.CountAsync(keyword: search, categoryId: categoryId, newsStatus: status, createdById: createdById, startDate: startDate, endDate: endDate);
        }

        public async Task<NewsArticle?> GetNewsById(string id)
        {
            return await _newsArticleRepository.GetByIdAsync(id, default, x => x.Category!, x => x.Tags, x => x.CreatedBy!);
        }

        public async Task<List<NewsArticle>> GetRelatedNews(string currentArticleId, List<int> tagIds)
        {
            return await _newsArticleRepository.GetRelatedNewsAsync(currentArticleId, tagIds);
        }

        public async Task AddNewsArticle(NewsArticle article, List<int> tagIds, Microsoft.AspNetCore.Http.IFormFile? imageFile = null)
        {
            // Generate ID if not set
            if (string.IsNullOrEmpty(article.Id))
            {
                article.Id = Guid.NewGuid().ToString().Substring(0, 20);
            }

            // Handle image upload
            if (imageFile != null && imageFile.Length > 0)
            {
                var uploadResults = await _fileStorageService.UploadAsync(new List<Microsoft.AspNetCore.Http.IFormFile> { imageFile });
                if (uploadResults.Any())
                {
                    article.NewsArticleImage = uploadResults.First();
                }
            }

            article.CreatedDate = DateTime.Now;
            article.ModifiedDate = DateTime.Now;

            if (tagIds != null && tagIds.Any())
            {
                var tags = await _tagRepository.Query()
                    .Where(t => tagIds.Contains(t.Id))
                    .ToListAsync();
                article.Tags = tags;
            }

            await _newsArticleRepository.AddAsync(article);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateNewsArticle(NewsArticle article, List<int> tagIds, IFormFile? imageFile = null)
        {
            var existingArticle = await _newsArticleRepository.GetByIdAsync(article.Id, default, x => x.Tags);
            if (existingArticle == null) return;

            if (imageFile != null && imageFile.Length > 0)
            {
                if (!string.IsNullOrEmpty(existingArticle.NewsArticleImage))
                {
                    await _fileStorageService.DeleteAsync(existingArticle.NewsArticleImage);
                }

                var uploadResults = await _fileStorageService.UploadAsync(new List<Microsoft.AspNetCore.Http.IFormFile> { imageFile });
                if (uploadResults.Any())
                {
                    article.NewsArticleImage = uploadResults.First();
                }
            }
            else if (article.NewsArticleImage != existingArticle.NewsArticleImage && !string.IsNullOrEmpty(existingArticle.NewsArticleImage))
            {
                await _fileStorageService.DeleteAsync(existingArticle.NewsArticleImage);
            }

            _mapper.Map(article, existingArticle);
            existingArticle.ModifiedDate = DateTime.Now;

            existingArticle.Tags.Clear();
            if (tagIds != null && tagIds.Any())
            {
                var tags = await _tagRepository.Query()
                    .Where(t => tagIds.Contains(t.Id))
                    .ToListAsync();
                existingArticle.Tags = tags;
            }

            await _newsArticleRepository.UpdateAsync(existingArticle);
            await _unitOfWork.SaveChangesAsync();
        }
        public async Task DeleteNewsArticle(string id)
        {
            var article = await _newsArticleRepository.GetByIdAsync(id);
            if (article != null)
            {
                if (!string.IsNullOrEmpty(article.NewsArticleImage))
                {
                    await _fileStorageService.DeleteAsync(article.NewsArticleImage);
                }
                await _newsArticleRepository.DeleteAsync(article);
                await _unitOfWork.SaveChangesAsync();
            }
        }
    }
}
