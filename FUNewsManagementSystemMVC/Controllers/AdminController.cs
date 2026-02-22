using FUNewsManagementSystemMVC.Models;
using Microsoft.AspNetCore.Mvc;
using Services.Abstractions;
using BusinessObject.Entities;
using AutoMapper;
using BusinessObject.Enums;
using Services.Helpers;

namespace FUNewsManagementSystemMVC.Controllers
{
    public class AdminController : Controller
    {
        private readonly ISystemAccountService _systemAccountService;
        private readonly INewsArticleService _newsArticleService;
        private readonly IMapper _accountMapper;

        public AdminController(ISystemAccountService systemAccountService, INewsArticleService newsArticleService, IMapper accountMapper)
        {
            _systemAccountService = systemAccountService;
            _newsArticleService = newsArticleService;
            _accountMapper = accountMapper;
        }

        public async Task<IActionResult> Reports(DateTime? startDate, DateTime? endDate)
        {
            ViewBag.StartDate = startDate?.ToString("yyyy-MM-dd");
            ViewBag.EndDate = endDate?.ToString("yyyy-MM-dd");

            var news = await _newsArticleService.GetAllNews(startDate: startDate, endDate: endDate);
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
                var newAccount = _accountMapper.Map<SystemAccount>(model);
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
