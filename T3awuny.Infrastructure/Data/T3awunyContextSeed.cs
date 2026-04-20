using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using T3awuny.Core.Entities;

namespace T3awuny.Infrastructure.Data
{
    public static class T3awunyContextSeed
    {
        public static async Task SeedRolesAsync(T3awunyDbContext _dbContext)
        {
                if (!_dbContext.Roles.Any())
                {
                    _dbContext.Roles.AddRange(
                        new IdentityRole { Name = "Admin", NormalizedName = "ADMIN" },
                        new IdentityRole { Name = "Farmer", NormalizedName = "FARMER" },
                        new IdentityRole { Name = "Trader", NormalizedName = "TRADER" }
                    );
                    await _dbContext.SaveChangesAsync();
            }
        }

        public static async Task SeedAdminAsync(UserManager<ApplicationUser> userManager)
        {
            if (!userManager.Users.Any())
            {
                var admin = new ApplicationUser
                {
                    Email = "Admin@gmail.com",
                    UserName = "admin1",
                    Name = "Admin1",
                    Addresses = new List<Address> { new Address { Street = "التعاونيات", City = "الفيوم", Governorate = "الفيوم", Country = "مصر" } },
                    IsActive = true,
                    IsVerified = true,
                    EmailConfirmed = true,
                };
                await userManager.CreateAsync(admin, "Admin@123");
                await userManager.AddToRoleAsync(admin, "Admin");
            }
        }
    }
}
