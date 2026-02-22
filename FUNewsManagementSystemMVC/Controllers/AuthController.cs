using FUNewsManagementSystemMVC.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;
using Services.Abstractions;
using IAuthenticationService = Services.Abstractions.IAuthenticationService;

namespace FUNewsManagementSystemMVC.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly ISystemAccountService _systemAccountService;
        private readonly IConfiguration _configuration;

        public AuthController(
            IAuthenticationService authenticationService, 
            ISystemAccountService systemAccountService,
            IConfiguration configuration)
        {
            _authenticationService = authenticationService;
            _systemAccountService = systemAccountService;
            _configuration = configuration;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var adminEmail = _configuration["AdminAccount:Email"];
            var adminPassword = _configuration["AdminAccount:Password"];

            if (model.Email.Equals(adminEmail, StringComparison.OrdinalIgnoreCase) && model.Password == adminPassword)
            {
                HttpContext.Session.SetString("AccountId", "0");
                HttpContext.Session.SetString("AccountName", "Administrator");
                HttpContext.Session.SetString("AccountEmail", adminEmail);
                HttpContext.Session.SetString("Role", "Admin");
                return RedirectToAction("Dashboard", "Admin");
            }

            var account = await _authenticationService.Login(model.Email, model.Password);
            if (account == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid email or password");
                return View(model);
            }

            HttpContext.Session.SetString("AccountId", account.Id.ToString());
            HttpContext.Session.SetString("AccountName", account.AccountName ?? "User");
            HttpContext.Session.SetString("AccountEmail", account.AccountEmail ?? "");
            HttpContext.Session.SetString("Role", account.AccountRole.ToString() ?? "");

            if (account.AccountRole == BusinessObject.Enums.AccountRole.Staff)
            {
                return RedirectToAction("Dashboard", "Staff");
            }
            else if (account.AccountRole == BusinessObject.Enums.AccountRole.Lecturer)
            {
                return RedirectToAction("Index", "Home");
            }
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            AuthenticationHttpContextExtensions.SignOutAsync(HttpContext, CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult ExternalLogin()
        {
            var properties = new AuthenticationProperties
            {
                RedirectUri = Url.Action("GoogleResponse")
            };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        [HttpGet]
        public async Task<IActionResult> GoogleResponse()
        {
            var result = await AuthenticationHttpContextExtensions.AuthenticateAsync(HttpContext, CookieAuthenticationDefaults.AuthenticationScheme);

            if (!result.Succeeded)
                return RedirectToAction("Login");

            var email = result.Principal.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;

            if (string.IsNullOrEmpty(email))
            {
                await AuthenticationHttpContextExtensions.SignOutAsync(HttpContext, CookieAuthenticationDefaults.AuthenticationScheme);
                TempData["ErrorMessage"] = "Could not retrieve email from Google account.";
                return RedirectToAction("Login");
            }

            var account = await _systemAccountService.GetAccountByEmail(email);

            if (account == null)
            {
                await AuthenticationHttpContextExtensions.SignOutAsync(HttpContext, CookieAuthenticationDefaults.AuthenticationScheme);
                TempData["ErrorMessage"] = "This account is not authorized to log in to the system. Please contact admin.";
                return RedirectToAction("Login");
            }

            // Set Session
            HttpContext.Session.SetString("AccountId", account.Id.ToString());
            HttpContext.Session.SetString("AccountName", account.AccountName ?? "User");
            HttpContext.Session.SetString("AccountEmail", account.AccountEmail ?? "");
            HttpContext.Session.SetString("Role", account.AccountRole.ToString() ?? "");

            if (account.AccountRole == BusinessObject.Enums.AccountRole.Staff)
            {
                return RedirectToAction("Dashboard", "Staff");
            }
            else if (account.AccountRole == BusinessObject.Enums.AccountRole.Lecturer)
            {
                return RedirectToAction("Index", "Home");
            }

            return RedirectToAction("Index", "Home");
        }
    }
}
