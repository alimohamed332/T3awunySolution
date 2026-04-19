using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using T3awuny.Core.Entities;

namespace T3awuny.Core.Repository.Contracts
{
    public interface IUserRepository
    {
        Task<IReadOnlyList<ApplicationUser>> GetAllVerifiedUsersAsync();
        Task<IReadOnlyList<ApplicationUser>> GetAllNonVerifiedUsersAsync();
        Task<IReadOnlyList<ApplicationUser>> GetBannedUsersAsync();
    }
}
