using HumanResources.Application.AuthServices;
using HumanResources.Domain.Entities;
using HumanResources.Web.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using static HumanResources.Application.Dtos.AuthDto;

namespace HumanResources.Web.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(Login model)
        {
            bool IsAuthenticated = false;

            if (ModelState.IsValid)
            {
                var login = new Login
                {
                    Email = model.Email,
                    Password = model.Password,
                };
                var result = await _authService.LoginAsync(login);

                if (result==true)
                {
                    GlobalVariables.IsAuthenticated = true;
                    // Redirect to the desired page on success
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    // Handle failure
                    IsAuthenticated = false;

                    ModelState.AddModelError(string.Empty, "خطأ فى اسم المستخدم او كلمة السر");
                    return View(model);
                }
            }

            return View(model);
        }
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(Register model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _authService.RegisterAsync(model);

            if (result.Succeeded)
            {
                return RedirectToAction("Login", "Auth"); // Redirect on successful registration
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model); // Return the form with validation errors
        }
        // Logout action
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _authService.LogoutAsync();
            GlobalVariables.IsAuthenticated = false;

            return RedirectToAction("Login");
        }

        // Access Denied
        public IActionResult AccessDenied()
        {
            return View();
        }

    }
}