using AspNetCore.ReportingServices.ReportProcessing.ReportObjectModel;
using HumanResources.Domain.Entities;
using HumanResources.Web.Helpers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using static HumanResources.Application.Dtos.AuthDto;

namespace HumanResources.Web.Controllers
{

    public class AuthController : Controller
    {

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AuthController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // Register GET
        [HttpGet]
        public IActionResult Register() => View();

        // Register POST
        [HttpPost]
        public async Task<IActionResult> Register(Register model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { Name=model.Name,UserName = model.Email, Email = model.Email };
                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Login", "Auth");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(model);
        }

        // Login GET
        [HttpGet]
        public IActionResult Login() => View();

        // Login POST
        [HttpPost]
        public async Task<IActionResult> Login(Login model)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, isPersistent: false, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    // Retrieve the user by email
                    var user = await _userManager.FindByEmailAsync(model.Email);
                    {
                        // Issue authentication cookie with username claim
                        var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Name), // Add the username as a claim
                    new Claim(ClaimTypes.Email, user.Email) // Optional: Add email as a claim
                };

                        var identity = new ClaimsIdentity(claims, "CookieAuth");
                        var principal = new ClaimsPrincipal(identity);

                        await HttpContext.SignInAsync("CookieAuth", principal);

                        return RedirectToAction("Index", "Home");
                    }
                }


                    ModelState.AddModelError("", "Invalid login attempt.");
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("CookieAuth");
            return RedirectToAction("Login", "Auth");
        }

    }
}