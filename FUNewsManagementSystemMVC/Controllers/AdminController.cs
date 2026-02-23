using FUNewsManagementSystemMVC.Models;
using Microsoft.AspNetCore.Mvc;
using Services.Abstractions;
using BusinessObject.Entities;
using AutoMapper;
using BusinessObject.Enums;
using Services.Helpers;

using Microsoft.AspNetCore.Authorization;

namespace FUNewsManagementSystemMVC.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ISystemAccountService _systemAccountService;
        private readonly INewsArticleService _newsArticleService;
        private readonly ICategoryService _categoryService;
        private readonly ITagService _tagService;
        private readonly IMapper _mapper;

        public AdminController(
            ISystemAccountService systemAccountService, 
            INewsArticleService newsArticleService, 
            ICategoryService categoryService,
            ITagService tagService,
            IMapper mapper)
        {
            _systemAccountService = systemAccountService;
            _newsArticleService = newsArticleService;
            _categoryService = categoryService;
            _tagService = tagService;
            _mapper = mapper;
        }

        public async Task<IActionResult> EditNews(string id)
        {
            var article = await _newsArticleService.GetNewsById(id);
            if (article == null) return NotFound();

            var model = _mapper.Map<UpdateNewsArticleViewModel>(article);
            
            ViewBag.Categories = await _categoryService.GetAllCategories(true);
            ViewBag.Tags = await _tagService.GetAllTags();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditNews(UpdateNewsArticleViewModel model)
        {
            if (ModelState.IsValid)
            {
                var article = _mapper.Map<NewsArticle>(model);
                article.Id = model.NewsArticleId;

                await _newsArticleService.UpdateNewsArticle(article, model.TagIds, model.ImageFile);
                TempData["SuccessMessage"] = "Article updated successfully.";
                return RedirectToAction(nameof(News));
            }

            ViewBag.Categories = await _categoryService.GetAllCategories(true);
            ViewBag.Tags = await _tagService.GetAllTags();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteNews(string id)
        {
            await _newsArticleService.DeleteNewsArticle(id);
            TempData["SuccessMessage"] = "Article deleted successfully.";
            return RedirectToAction(nameof(News));
        }

        public async Task<IActionResult> News(string? search = null, short? categoryId = null, bool? status = null, int page = 1)
        {
            int pageSize = 10;
            var news = await _newsArticleService.GetAllNews(search, categoryId, status, pageIndex: page, pageSize: pageSize);
            var categories = await _categoryService.GetAllCategories(true);
            var totalItems = await _newsArticleService.GetNewsCount(search, categoryId, status);

            ViewBag.Search = search;
            ViewBag.CategoryId = categoryId;
            ViewBag.Status = status;
            ViewBag.Categories = categories;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)System.Math.Ceiling(totalItems / (double)pageSize);
            ViewBag.TotalItems = totalItems;

            return View(news);
        }

        public async Task<IActionResult> CreateNews()
        {
            var categories = await _categoryService.GetAllCategories(true);
            var tags = await _tagService.GetAllTags();

            ViewBag.Categories = categories;
            ViewBag.Tags = tags;

            return View(new CreateNewsArticleViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> CreateNews(CreateNewsArticleViewModel model)
        {
            if (ModelState.IsValid)
            {
                var article = _mapper.Map<NewsArticle>(model);
                var accountIdStr = HttpContext.Session.GetString("AccountId");
                if (short.TryParse(accountIdStr, out short accountId))
                {
                    article.CreatedById = accountId;
                }

                await _newsArticleService.AddNewsArticle(article, model.TagIds, model.ImageFile);
                TempData["SuccessMessage"] = "Article created successfully.";
                return RedirectToAction(nameof(News));
            }

            ViewBag.Categories = await _categoryService.GetAllCategories(true);
            ViewBag.Tags = await _tagService.GetAllTags();
            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> CreateTag([FromBody] CreateTagViewModel model)
        {
            if (!ModelState.IsValid) return BadRequest("Invalid data");

            var existingTag = await _tagService.GetTagByName(model.TagName);
            if (existingTag != null) return BadRequest("Tag already exists");

            var tag = new Tag { TagName = model.TagName, Note = model.Note };
            await _tagService.AddTag(tag);

            return Json(new { id = tag.Id, tagName = tag.TagName });
        }

        public async Task<IActionResult> Reports(DateTime? startDate, DateTime? endDate, int page = 1)
        {
            int pageSize = 10;
            ViewBag.StartDate = startDate?.ToString("yyyy-MM-dd");
            ViewBag.EndDate = endDate?.ToString("yyyy-MM-dd");

            var news = await _newsArticleService.GetAllNews(startDate: startDate, endDate: endDate, pageIndex: page, pageSize: pageSize);
            var totalItems = await _newsArticleService.GetNewsCount(startDate: startDate, endDate: endDate);
            
            var allNewsForStats = await _newsArticleService.GetAllNews(startDate: startDate, endDate: endDate);
            ViewBag.AllNewsForStats = allNewsForStats;

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)System.Math.Ceiling(totalItems / (double)pageSize);
            ViewBag.TotalItems = totalItems;

            return View(news);
        }

        public async Task<IActionResult> Dashboard()
        {
            var now = DateTime.Now;
            var firstDayOfMonth = new DateTime(now.Year, now.Month, 1);
            var firstDayOfLastMonth = firstDayOfMonth.AddMonths(-1);
            var lastDayOfLastMonth = firstDayOfMonth.AddDays(-1);

            // Fetch News Statistics
            var totalNews = await _newsArticleService.GetNewsCount();
            var newsThisMonth = await _newsArticleService.GetNewsCount(startDate: firstDayOfMonth);
            var newsLastMonth = await _newsArticleService.GetNewsCount(startDate: firstDayOfLastMonth, endDate: lastDayOfLastMonth);
            var activeNews = await _newsArticleService.GetNewsCount(status: true);

            // Fetch Account Statistics
            var accounts = await _systemAccountService.GetAllAccounts();
            var totalAccounts = accounts.Count;

            // Calculate News Growth %
            double newsPercentChange = 0;
            if (newsLastMonth > 0)
            {
                newsPercentChange = ((double)(newsThisMonth - newsLastMonth) / newsLastMonth) * 100;
            }
            else if (newsThisMonth > 0)
            {
                newsPercentChange = 100;
            }

            // Calculations for display
            ViewBag.TotalNews = totalNews;
            ViewBag.ActiveNews = activeNews;
            ViewBag.NewsPercentChange = newsPercentChange;
            ViewBag.TotalAccounts = totalAccounts;
            
            // Mocked account growth since SystemAccount lacks CreatedDate
            ViewBag.AccountsPercentChange = 12.5; 

            return View();
        }

        public async Task<IActionResult> Accounts(string? search, int? role)
        {
            ViewBag.Search = search;
            ViewBag.Role = role;

            AccountRole? accountRole = role.HasValue ? (AccountRole)role.Value : null;
            var accounts = await _systemAccountService.GetAllAccounts(accountRole, search);
            
            return View(accounts);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAccount(CreateAccountViewModel model)
        {
            if (ModelState.IsValid)
            {
                var newAccount = _mapper.Map<SystemAccount>(model);
                await _systemAccountService.AddAccount(newAccount);

                return RedirectToAction("Accounts");
            }
            var accounts = await _systemAccountService.GetAllAccounts();
            return View("Accounts", accounts); 
        }

        [HttpPost]
        public async Task<IActionResult> UpdateAccount(EditAccountViewModel model)
        {
            if (ModelState.IsValid)
            {
                var existingAccount = await _systemAccountService.GetAccountById(model.Id);
                if (existingAccount != null)
                {
                    existingAccount.AccountName = model.AccountName;
                    existingAccount.AccountEmail = model.AccountEmail;
                    existingAccount.AccountRole = model.AccountRole;

                    if (!string.IsNullOrEmpty(model.AccountPassword))
                    {
                        existingAccount.AccountPassword = PasswordHelper.HashPassword(model.AccountPassword);
                    }

                    await _systemAccountService.UpdateAccount(existingAccount);
                    return RedirectToAction("Accounts");
                }
            }
            var accounts = await _systemAccountService.GetAllAccounts();
            return View("Accounts", accounts);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteAccount(short id)
        {
            await _systemAccountService.DeleteAccount(id);
            return RedirectToAction("Accounts");
        }

        public async Task<IActionResult> Categories(string? search = null, bool? isActive = null)
        {
            var categories = await _categoryService.GetAllCategories(isActive, search);
            ViewBag.Search = search;
            ViewBag.IsActive = isActive;
            ViewBag.CategoriesForParent = await _categoryService.GetAllCategories(true);
            return View(categories);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCategory(CreateCategoryViewModel model)
        {
            if (ModelState.IsValid)
            {
                var category = _mapper.Map<Category>(model);
                await _categoryService.AddCategory(category);
                TempData["SuccessMessage"] = "Category created successfully.";
                return RedirectToAction(nameof(Categories));
            }
            var categories = await _categoryService.GetAllCategories();
            ViewBag.CategoriesForParent = await _categoryService.GetAllCategories(true);
            return View("Categories", categories);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateCategory(UpdateCategoryViewModel model)
        {
            if (ModelState.IsValid)
            {
                var category = _mapper.Map<Category>(model);
                await _categoryService.UpdateCategory(category);
                TempData["SuccessMessage"] = "Category updated successfully.";
                return RedirectToAction(nameof(Categories));
            }
            var categories = await _categoryService.GetAllCategories();
            return View("Categories", categories);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCategory(short id)
        {
            var newsCount = await _newsArticleService.GetNewsCount(categoryId: id);
            if (newsCount > 0)
            {
                TempData["ErrorMessage"] = "Cannot delete category because it has associated news articles.";
                return RedirectToAction(nameof(Categories));
            }
            await _categoryService.DeleteCategory(id);
            TempData["SuccessMessage"] = "Category deleted successfully.";
            return RedirectToAction(nameof(Categories));
        }

        public IActionResult Profile()
        {
            return View();
        }
    }
}
