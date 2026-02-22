using BusinessObject.Entities;
using Microsoft.AspNetCore.Mvc;
using Services.Abstractions;
using System.Threading.Tasks;
using AutoMapper;
using FUNewsManagementSystemMVC.Models;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System.Linq;

using Microsoft.AspNetCore.Authorization;

namespace FUNewsManagementSystemMVC.Controllers
{
    [Authorize(Roles = "Staff")]
    public class StaffController : Controller
    {
        private readonly ICategoryService _categoryService;
        private readonly INewsArticleService _newsArticleService;
        private readonly ITagService _tagService;
        private readonly ISystemAccountService _systemAccountService;
        private readonly IMapper _mapper;

        public StaffController(
            ICategoryService categoryService, 
            INewsArticleService newsArticleService, 
            ITagService tagService,
            ISystemAccountService systemAccountService,
            IMapper mapper)
        {
            _categoryService = categoryService;
            _newsArticleService = newsArticleService;
            _tagService = tagService;
            _systemAccountService = systemAccountService;
            _mapper = mapper;
        }

        public IActionResult Dashboard()
        {
            return View();
        }

        public async Task<IActionResult> Categories(string? search = null, bool? isActive = null)
        {
            var categories = await _categoryService.GetAllCategories(isActive, search);
            ViewBag.Search = search;
            ViewBag.IsActive = isActive;
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
                return RedirectToAction(nameof(News));
            }

            ViewBag.Categories = await _categoryService.GetAllCategories(true);
            ViewBag.Tags = await _tagService.GetAllTags();
            return View(model);
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

        [HttpPost]
        public async Task<IActionResult> Preview(PreviewNewsViewModel model)
        {
            var article = new NewsArticle
            {
                NewsTitle = model.NewsTitle,
                Headline = model.Headline,
                NewsContent = model.NewsContent,
                NewsSource = model.NewsSource,
                NewsArticleImage = model.NewsArticleImage,
                CreatedDate = DateTime.Now
            };

            if (model.CategoryId > 0)
            {
                article.Category = await _categoryService.GetCategoryById(model.CategoryId);
            }

            if (model.TagIds != null && model.TagIds.Any())
            {
                var allTags = await _tagService.GetAllTags();
                article.Tags = allTags.Where(t => model.TagIds.Contains(t.Id)).ToList();
            }

            article.CreatedBy = new SystemAccount
            {
                AccountName = HttpContext.Session.GetString("AccountName") ?? "Staff Member"
            };

            if (model.ImageFile != null && model.ImageFile.Length > 0)
            {
                using var ms = new System.IO.MemoryStream();
                await model.ImageFile.CopyToAsync(ms);
                var fileBytes = ms.ToArray();
                string s = Convert.ToBase64String(fileBytes);
                article.NewsArticleImage = "data:" + model.ImageFile.ContentType + ";base64," + s;
            }

            return View(article);
        }

        [HttpPost]
        public async Task<IActionResult> CreateTag([FromBody] CreateTagViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var errors = string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return BadRequest(errors);
            }

            var existingTag = await _tagService.GetTagByName(model.TagName);
            if (existingTag != null)
            {
                return BadRequest("Tag with this name already exists");
            }

            var tag = new Tag 
            { 
                TagName = model.TagName,
                Note = model.Note
            };
            await _tagService.AddTag(tag);

            return Json(new { id = tag.Id, tagName = tag.TagName });
        }

        public async Task<IActionResult> History(string? search = null, short? categoryId = null, bool? status = null)
        {
            var accountIdStr = HttpContext.Session.GetString("AccountId");
            if (short.TryParse(accountIdStr, out short accountId))
            {
                var news = await _newsArticleService.GetAllNews(search, categoryId, status, accountId);
                var categories = await _categoryService.GetAllCategories(true);

                ViewBag.Search = search;
                ViewBag.CategoryId = categoryId;
                ViewBag.Status = status;
                ViewBag.Categories = categories;
                ViewBag.IsHistory = true;

                return View("History", news);
            }
            return RedirectToAction("Login", "Auth");
        }

        public async Task<IActionResult> Profile()
        {
            var accountIdStr = HttpContext.Session.GetString("AccountId");
            if (short.TryParse(accountIdStr, out short accountId))
            {
                var account = await _systemAccountService.GetAccountById(accountId);
                if (account != null)
                {
                    var model = _mapper.Map<StaffProfileViewModel>(account);
                    model.JoinedDate = DateTime.Now.AddYears(-2);
                    model.LastLogin = DateTime.Now;
                    model.Department = "Academic Office";
                    model.Status = "Active";
                    return View(model);
                }
            }
            return RedirectToAction("Login", "Auth");
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProfile(StaffProfileViewModel model)
        {
            if (ModelState.IsValid)
            {
                var accountIdStr = HttpContext.Session.GetString("AccountId");
                if (short.TryParse(accountIdStr, out short accountId))
                {
                    var account = await _systemAccountService.GetAccountById(accountId);
                    if (account != null)
                    {
                        // Handle password update if provided
                        if (!string.IsNullOrEmpty(model.NewPassword))
                        {
                            if (string.IsNullOrEmpty(model.CurrentPassword))
                            {
                                ModelState.AddModelError("CurrentPassword", "Current password is required to change password");
                                return View("Profile", model);
                            }

                            if (account.AccountPassword != model.CurrentPassword)
                            {
                                ModelState.AddModelError("CurrentPassword", "Incorrect current password");
                                return View("Profile", model);
                            }

                            account.AccountPassword = model.NewPassword;
                        }

                        _mapper.Map(model, account);
                        
                        await _systemAccountService.UpdateAccount(account);
                        
                        // Update session
                        HttpContext.Session.SetString("AccountName", account.AccountName ?? "User");
                        
                        TempData["SuccessMessage"] = "Profile updated successfully";
                        return RedirectToAction(nameof(Profile));
                    }
                }
            }
            return View("Profile", model);
        }
    }
}
