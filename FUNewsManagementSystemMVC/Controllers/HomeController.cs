using FUNewsManagementSystemMVC.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Services.Abstractions;

namespace FUNewsManagementSystemMVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly INewsArticleService _newsArticleService;
        private readonly ICategoryService _categoryService;

        public HomeController(INewsArticleService newsArticleService, ICategoryService categoryService)
        {
            _newsArticleService = newsArticleService;
            _categoryService = categoryService;
        }

        public async Task<IActionResult> Index(string? search, short? categoryId, int page = 1)
        {
            int pageSize = 10;
            var news = await _newsArticleService.GetAllNews(search: search, categoryId: categoryId, status: true, pageIndex: page, pageSize: pageSize);
            
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_NewsListPartial", news);
            }

            var categories = await _categoryService.GetAllCategories(true);
            
            ViewBag.Search = search;
            ViewBag.CategoryId = categoryId;
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
