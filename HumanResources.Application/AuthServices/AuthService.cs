using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HumanResources.Application.Dtos.AuthDto;

namespace HumanResources.Application.AuthServices
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public AuthService(UserManager<IdentityUser> userManager
            , SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<SignInResult> LoginAsync(Login login)
        {
            // Find the user by email
            var user = await _userManager.FindByEmailAsync(login.Email);

            if (user == null)
            {
                // Handle user not found (could return a custom result or throw an exception)
                return SignInResult.Failed;
            }

            // Sign in the user
            var result = await _signInManager.PasswordSignInAsync(user.UserName, login.Password, login.RememberMe, lockoutOnFailure: false);
            return result;
        }

        public async Task<IdentityResult> RegisterAsync(Register request)
        {
            var user = new IdentityUser { UserName = request.Email, Email = request.Email };
            var result = await _userManager.CreateAsync(user, request.Password);
            return result;
        }
    }

}
