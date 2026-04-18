using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using T3awuny.Application.DTOs.User;
using T3awuny.Core.Entities;

namespace T3awuny.Application.Contracts
{
    public interface IUserService
    {
        Task<string> GetUserIdByEmailAsync(string email);
        Task<ApplicationUser?> GetUserByEmailAsync(string email);

        Task<IEnumerable<UserDetailsDto>> GetAllVerifiedUsersAsync();
        Task<IEnumerable<UserDetailsDto>> GetAllNonVerifiedUsersAsync();
        Task<bool> UpdateProfileImageAsync(string userId, IFormFile image);
    }
}
