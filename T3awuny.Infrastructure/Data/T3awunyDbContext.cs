using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using T3awuny.Core.Entities;
using T3awuny.Infrastructure.Configurations;

namespace T3awuny.Infrastructure.Data
{
    public class T3awunyDbContext : IdentityDbContext<ApplicationUser>
    {
        public virtual DbSet<FarmerProfile> FarmerProfiles { get; set; }
        public virtual DbSet<TraderProfile> TraderProfiles { get; set; }
        public virtual DbSet<Address> Addresses { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<ProductImage> ProductImages { get; set; }

        public T3awunyDbContext(DbContextOptions<T3awunyDbContext> options) : base(options) { }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            //builder.ApplyConfiguration(new ApplicationUserConfigurations()); //and so on with all or
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            base.OnModelCreating(builder);
            
        }
    }
}
