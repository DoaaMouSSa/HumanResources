using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HumanResources.Application.Dtos.AuthDto;

namespace HumanResources.Application.AuthServices
{
    public interface IAuthService
    {
        Task<IdentityResult> RegisterAsync(Register register);
        Task<bool> LoginAsync(Login login);
        Task LogoutAsync();
    }
}
