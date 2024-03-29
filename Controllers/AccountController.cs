﻿using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Fakebook.Models;
using AccountService = Fakebook.Services.AccountService;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using System.Threading;
using System.Security.Principal;
using Fakebook.Services;

namespace Fakebook.Controllers
{
    public class AccountController : Controller
    {
        private readonly AccountService accountService;

        public AccountController(AccountService accountService)
        {
            this.accountService = accountService;
        }

        // Used for directing to login page
        [HttpGet("/Account/Login", Name = "Login")]
        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "User", new { id = User.Identity.GetPersonId() });
            }
            else
            {
                return View();
            }
        }

        // Used for logging into the site
        [HttpPost("/Account/Login", Name = "LoginPost")]
        public async Task<IActionResult> Login(string username, string password, string returnUrl)
        {
            var identity = accountService.Authenticate(username, password);
            if (identity == null)   
                return RedirectToAction(nameof(Login));

            var claimsPrincipal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal, new AuthenticationProperties());

            return string.IsNullOrWhiteSpace(returnUrl) ? RedirectToAction("RedirectToIndex", "User") : (IActionResult)LocalRedirect(returnUrl);
        }

        // Used for logging out and clearing cookies
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction(nameof(Login));
        }

        // Used for directing to register page
        [HttpGet("/Account/Register", Name = "Register")]
        public IActionResult Register()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "User", new { id = User.Identity.GetPersonId()});
            }
            else
            {
                return View();
            }
        }

        // Used for registering the user and directing to login page afterwards
        [HttpPost("/Account/Register", Name = "RegisterPost")]
        public IActionResult Register(Person person)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "User", new { id = User.Identity.GetPersonId() });
            }
            else if (ModelState.IsValid)
            {
                bool successfulRegister = accountService.AddPerson(person);
                if (successfulRegister == true)
                {
                    return RedirectToAction(nameof(Login));
                }
            }
            return View(person);
        }

        public IActionResult AccessDenied()
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
