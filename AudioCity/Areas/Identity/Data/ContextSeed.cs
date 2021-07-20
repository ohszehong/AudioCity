using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AudioCity.Areas.Identity.Data
{
    public class ContextSeed
    {
        public static async Task SeedAdminAsync(UserManager<AudioCityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            //Seed Default User
            var admin = new AudioCityUser
            {
                UserName = "admin123@mail.com",
                Email = "admin123@mail.com",
                FullName = "Lim Ye Han",
                Dob = DateTime.Parse("12/2/1999"),
                ContactNo = "0123333333",
                Role = "Admin",
                EmailConfirmed = true,
            };
            if (userManager.Users.All(u => u.Id != admin.Id))
            {
                var user = await userManager.FindByEmailAsync(admin.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(admin, "Admin@123");
                    await userManager.AddToRoleAsync(admin, Roles.Admin.ToString());
                }

            }
        }
    }
}
