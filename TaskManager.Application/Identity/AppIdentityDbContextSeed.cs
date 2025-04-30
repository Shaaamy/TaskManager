using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Core.Entities.Identity;

namespace TaskManager.Repository.Identity
{
    public static class AppIdentityDbContextSeed
    {
        public static async Task SeedUserAsync(UserManager<AppUser> userManager)
        {
            if(!userManager.Users.Any())
            {
                var User = new AppUser()
                {
                    DisplayName = "Amr Shamy",
                    Email = "amrshamy91@gmail.com",
                    UserName = "amrshamy",
                    PhoneNumber = "01100156132"
                };
                await userManager.CreateAsync(User, "Pa$$w0rd");
            }
            
        }
    }
}
