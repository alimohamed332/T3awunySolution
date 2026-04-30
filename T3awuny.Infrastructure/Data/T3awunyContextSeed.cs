using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using T3awuny.Core.Entities;
using T3awuny.Core.Entities.UserModule;

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

        public static async Task SeedCategoriesAsync(T3awunyDbContext _dbContext)
        {
            if (!_dbContext.Categories.Any())
            {
                // Parent categories
                _dbContext.Categories.AddRange(
                new Category { Name = "Vegetables", NameAr = "خضروات" },
                new Category { Name = "Fruits", NameAr = "فواكه" },
                new Category { Name = "Grains", NameAr = "حبوب" },
                new Category { Name = "Dairy", NameAr = "ألبان" },
                new Category { Name = "Livestock", NameAr = "مواشي" }
                );

               //await _dbContext.SaveChangesAsync();
                // Subcategories
                _dbContext.Categories.AddRange(
                    new Category { Name = "Leafy Greens", NameAr = "خضروات ورقية", ParentCategoryId = 1 },
                    new Category { Name = "Root Vegetables", NameAr = "جذور", ParentCategoryId = 1 },
                    new Category { Name = "Other", NameAr = "اخري", ParentCategoryId = 1 },
                    new Category { Name = "Citrus Fruits", NameAr = "حمضيات", ParentCategoryId = 2 },
                    new Category { Name = "Wheat", NameAr = "قمح", ParentCategoryId = 3 },
                    new Category { Name = "Rice", NameAr = "أرز", ParentCategoryId = 3 }
                );

                await _dbContext.SaveChangesAsync();
            }
                
        }
    }
}
