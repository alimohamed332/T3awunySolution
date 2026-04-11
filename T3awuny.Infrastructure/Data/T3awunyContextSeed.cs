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

        public static async Task SeedAdminAsync(T3awunyDbContext _dbContext)
        {
            if (!_dbContext.Users.Any())
            {
                _dbContext.Users.Add(
                    new ApplicationUser { 
                        Email = "Admin@gmail.com",
                        UserName = "admin1",
                        Name = "Admin1", 
                        Addresses = new List<Address> { new Address { Street = "Alfawal" ,City = "Fayoum" , Governorate = "Fayoum" , Country = "Egypt"} },   
                        PasswordHash = "69195a86ced09fc9f226ad02a9a56a7a3dcbfebd86f135f079c844f90299fefe"
                    }
                );
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}
