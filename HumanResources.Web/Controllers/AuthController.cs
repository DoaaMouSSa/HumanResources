using HumanResources.Application.AuthServices;
using HumanResources.Domain.Entities;
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
            if (ModelState.IsValid)
            {
                var login = new Login
                {
                    Email = model.Email,
                    Password = model.Password,
                    RememberMe = model.RememberMe
                };

                var result = await _authService.LoginAsync(login);

                if (result.Succeeded)
                {
                    // Redirect to the desired page on success
                    return RedirectToAction("Index", "Home");
                }
                else if (result.IsLockedOut)
                {
                    // Handle lockout
                    ModelState.AddModelError(string.Empty, "Account is locked out.");
                    return View(model);
                }
                else
                {
                    // Handle failure
                    ModelState.AddModelError(string.Empty, "خطأ فى اسم المستخدم او كلمة السر");
                    return View(model);
                }
            }

            // If we got this far, something failed, redisplay form
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
                return RedirectToAction("Index", "Home"); // Redirect on successful registration
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model); // Return the form with validation errors
        }
    }
}