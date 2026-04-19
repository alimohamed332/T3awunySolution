using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using T3awuny.Core.Entities;
using T3awuny.Core.Repository.Contracts;
using T3awuny.Infrastructure.Data;

namespace T3awuny.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly T3awunyDbContext _dbContext;

        public UserRepository(T3awunyDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IReadOnlyList<ApplicationUser>> GetAllVerifiedUsersAsync()
        {
            return await _dbContext.Users.Where(u => u.IsVerified).AsNoTracking().ToListAsync();
        }
        public async Task<IReadOnlyList<ApplicationUser>> GetAllNonVerifiedUsersAsync()
        {
            return await _dbContext.Users.Where(u => !u.IsVerified).AsNoTracking().ToListAsync();
        }

        public async Task<IReadOnlyList<ApplicationUser>> GetBannedUsersAsync()
        {
            return await _dbContext.Users.Where(u => !u.IsActive).AsNoTracking().ToListAsync();
        }
    }
}
