using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using T3awuny.Application.Contracts;
using T3awuny.Application.DTOs.Farmer;
using T3awuny.Application.DTOs.User;
using T3awuny.Core;
using T3awuny.Core.Entities;
using T3awuny.Core.Specifications;

namespace T3awuny.Application.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IFileStorageService _imageService;
        private readonly IUnitOfWork _unitOfWork;

        public UserService(UserManager<ApplicationUser> userManager, IFileStorageService imageService, IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _imageService = imageService;
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<UserDetailsDto>> GetAllNonVerifiedUsersAsync()
        {
            var users = await _unitOfWork.UserRepository.GetAllNonVerifiedUsersAsync();
            return users.Select(u => new UserDetailsDto
            {
                Id = u.Id,
                Name = u.Name,
                Email = u.Email!,
                UserName = u.UserName!,
                IsEmailConfirmed = u.EmailConfirmed
            });
        }

        public async Task<IEnumerable<UserDetailsDto>> GetAllVerifiedUsersAsync()
        {
            var users = await _unitOfWork.UserRepository.GetAllVerifiedUsersAsync();
            return users.Select(u => new UserDetailsDto
            {
                Id = u.Id,
                Name = u.Name,
                Email = u.Email!,
                UserName = u.UserName!,
                IsEmailConfirmed = u.EmailConfirmed
            });
        }

        public async Task<ApplicationUser?> GetUserByEmailAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            return user;
        }

        public async Task<string> GetUserIdByEmailAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            return user?.Id ?? string.Empty;
        }

        public async Task<bool> UpdateProfileImageAsync(string userId, IFormFile image)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user is null) return false;

            // Delete old image first
            if (!string.IsNullOrEmpty(user.ProfileImageUrl))
                _imageService.DeleteImage(user.ProfileImageUrl);

            // Save new image
            var url = await _imageService.SaveImageAsync(image, "users");
            user.ProfileImageUrl = url;

            await _userManager.UpdateAsync(user);
            return true;
        }
        
    }
}
