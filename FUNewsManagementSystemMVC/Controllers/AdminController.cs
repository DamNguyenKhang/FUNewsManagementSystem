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
                return RedirectToAction(nameof(Reports));
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
            return RedirectToAction(nameof(Reports));
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

        public IActionResult Dashboard()
        {
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

        public IActionResult Profile()
        {
            return View();
        }
    }
}
