using FUNewsManagementSystemMVC.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Services.Abstractions;
using BusinessObject.Entities;

namespace FUNewsManagementSystemMVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly INewsArticleService _newsArticleService;
        private readonly ICategoryService _categoryService;
        private readonly ITagService _tagService;

        public HomeController(INewsArticleService newsArticleService, ICategoryService categoryService, ITagService tagService)
        {
            _newsArticleService = newsArticleService;
            _categoryService = categoryService;
            _tagService = tagService;
        }

        public async Task<IActionResult> Index(string? search, short? categoryId, int? tagId, int page = 1)
        {
            int pageSize = 10;
            
            Tag? activeTag = null;
            if (tagId.HasValue)
            {
                activeTag = await _tagService.GetTagById(tagId.Value);
                if (activeTag != null)
                {
                    ViewBag.ActiveTag = activeTag;
                    string tagStr = "#" + activeTag.TagName;
                    if (string.IsNullOrEmpty(search))
                    {
                        search = tagStr;
                    }
                    else if (!search.Contains(tagStr))
                    {
                        search = tagStr + " " + search;
                    }
                }
            }

            // Clean search string for filtering (remove the hashtag if it matches the tagId filter)
            string? cleanSearch = search;
            if (activeTag != null)
            {
                cleanSearch = cleanSearch?.Replace("#" + activeTag.TagName, "").Trim();
            }

            var news = await _newsArticleService.GetAllNews(search: cleanSearch, categoryId: categoryId, status: true, tagId: tagId, pageIndex: page, pageSize: pageSize);
            
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_NewsListPartial", news);
            }

            var categories = await _categoryService.GetAllCategories(true);
            
            ViewBag.Search = search;
            ViewBag.CategoryId = categoryId;
            ViewBag.TagId = tagId;
            ViewBag.Categories = categories;
            ViewBag.Page = page;

            return View(news);
        }

        public async Task<IActionResult> Details(string id)
        {
            var article = await _newsArticleService.GetNewsById(id);
            if (article == null)
            {
                return NotFound();
            }

            var tagIds = article.Tags.Select(t => t.Id).ToList();
            var relatedNews = await _newsArticleService.GetRelatedNews(id, tagIds);
            ViewBag.RelatedNews = relatedNews;

            return View(article);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
