using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Core.Entities.Identity;

namespace TaskManager.Core.Services
{
    public interface ITokenService
    {
        Task<string> CreateTokenAsync(AppUser User, UserManager<AppUser> userManager,RoleManager<IdentityRole> roleManager);
    }
}
