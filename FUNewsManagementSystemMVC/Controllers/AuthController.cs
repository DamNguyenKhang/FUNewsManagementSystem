using FUNewsManagementSystemMVC.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;
using Services.Abstractions;
using System.Security.Claims;
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
                await SignInUser("0", "Administrator", adminEmail, "Admin");
                return RedirectToAction("Dashboard", "Admin");
            }

            var account = await _authenticationService.Login(model.Email, model.Password);
            if (account == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid email or password");
                return View(model);
            }

            string role = account.AccountRole.ToString();
            await SignInUser(account.Id.ToString(), account.AccountName ?? "User", account.AccountEmail ?? "", role);

            if (account.AccountRole == BusinessObject.Enums.AccountRole.Staff)
            {
                return RedirectToAction("Dashboard", "Staff");
            }
            return RedirectToAction("Index", "Home");
        }

        private async Task SignInUser(string id, string name, string email, string role)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, id),
                new Claim(ClaimTypes.Name, name),
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.Role, role)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(20)
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            HttpContext.Session.SetString("AccountId", id);
            HttpContext.Session.SetString("AccountName", name);
            HttpContext.Session.SetString("AccountEmail", email);
            HttpContext.Session.SetString("Role", role);
        }

        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Clear();
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
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
            var result = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);

            if (!result.Succeeded)
                return RedirectToAction("Login");

            var email = result.Principal.FindFirst(ClaimTypes.Email)?.Value;

            if (string.IsNullOrEmpty(email))
            {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                TempData["ErrorMessage"] = "Could not retrieve email from Google account.";
                return RedirectToAction("Login");
            }

            var account = await _systemAccountService.GetAccountByEmail(email);

            if (account == null)
            {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                TempData["ErrorMessage"] = "This account is not authorized to log in to the system. Please contact admin.";
                return RedirectToAction("Login");
            }

            string role = account.AccountRole.ToString();
            await SignInUser(account.Id.ToString(), account.AccountName ?? "User", account.AccountEmail ?? "", role);

            if (account.AccountRole == BusinessObject.Enums.AccountRole.Staff)
            {
                return RedirectToAction("Dashboard", "Staff");
            }

            return RedirectToAction("Index", "Home");
        }
    }
}
