using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities.Identity;

namespace Talabat.Repository.Identity
{
    public static class AppIdentityDbContextSeed
    {

        public static async Task SeedUserAsync(UserManager<AppUser> userManager)
        {
            if(!userManager.Users.Any())
            {
                var User = new AppUser()
                {
                    DisplayName = "kirollos samir",
                    Email = "kiko766@yahoo.com",
                    PhoneNumber = "01287897978",
                    UserName = "kiko766"
                };
                await userManager.CreateAsync(User,"Password1!");
            }
        }

    }
}
