using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using T3awuny.Application.Common;
using T3awuny.Application.Contracts;
using T3awuny.Application.DTOs.Address;
using T3awuny.Application.DTOs.Admin;
using T3awuny.Application.DTOs.Farmer;
using T3awuny.Application.DTOs.Trader;
using T3awuny.Application.DTOs.User;
using T3awuny.Application.Helpers;
using T3awuny.Core;
using T3awuny.Core.Entities;
using T3awuny.Core.Entities.AuctionModule;
using T3awuny.Core.Entities.Enums;
using T3awuny.Core.Entities.OrderAggregate;
using T3awuny.Core.Entities.ReviewModule;
using T3awuny.Core.Entities.UserModule;
using T3awuny.Core.Specifications;
using T3awuny.Core.Specifications.UserSpecs;

namespace T3awuny.Application.Services
{
    public class AdminService : IAdminService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly string _baseUrl;

        public AdminService(UserManager<ApplicationUser> userManager, IUnitOfWork unitOfWork, IMapper mapper, IConfiguration configuration)
        {
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _baseUrl = configuration["App:ApplicationUrl"] ?? "";
        }

        public async Task<ApiResponse<IReadOnlyList<FarmerProfileDto>>> GetPendingFarmersAsync()
        {
            var farmerSpecification = new FarmerSpecifications(f => !f.IsVerified); //user and address
            var farmerProfiles = await _unitOfWork.Repository<FarmerProfile>().GetAllWithSpecAsync(farmerSpecification);
            var farmerProfilesDtos = farmerProfiles.Select(f => _mapper.Map<FarmerProfileDto>(f));

            if (!farmerProfilesDtos.Any())
                return ApiResponse<IReadOnlyList<FarmerProfileDto>>.Fail("لا يوجد حسابات مزارعين في انتظار التحقق حاليا");
            farmerProfilesDtos.Select(fp => fp.ProfileImageUrl = $"{_baseUrl}{fp.ProfileImageUrl}");
            return ApiResponse<IReadOnlyList<FarmerProfileDto>>.Ok(farmerProfilesDtos.ToList(), "تم العثور على حسابات المزارعين في انتظار التحقق بنجاح");
        }

        public async Task<ApiResponse<IReadOnlyList<TraderProfileDto>>> GetPendingTradersAsync()
        {
            var traderSpecification = new TraderSpecifications(t => !t.IsVerified);// you can use only base spec but will not include user inside but it lighter if you didn't need it
            var traderProfiles = await _unitOfWork.Repository<TraderProfile>().GetAllWithSpecAsync(traderSpecification);
            var traderProfilesDtos = traderProfiles.Select(t => _mapper.Map<TraderProfileDto>(t));

            if (!traderProfilesDtos.Any()) 
                return ApiResponse<IReadOnlyList<TraderProfileDto>>.Fail("لا يوجد حسابات تجار في انتظار التحقق حاليا");
            traderProfilesDtos.Select(tp => tp.ProfileImageUrl = $"{_baseUrl}{tp.ProfileImageUrl}");
            return ApiResponse<IReadOnlyList<TraderProfileDto>>.Ok(traderProfilesDtos.ToList(), "تم العثور على حسابات التجار في انتظار التحقق بنجاح");
        }

        public async Task<ApiResponse<bool>> VerifyFarmerAsync(string farmerId)
        {
            var farmerSpecification = new FarmerSpecifications(f => f.FarmerId == farmerId);//user and defaul add
            var farmerProfile = await _unitOfWork.Repository<FarmerProfile>().GetByIdWithSpecAsync(farmerSpecification);
            if (farmerProfile is null)
                return ApiResponse<bool>.Fail("هذا المزارع لا يملك بروفايل ");

            farmerProfile.IsVerified = true;
            farmerProfile.VerifiedAt = DateTime.UtcNow;
            farmerProfile!.User!.IsVerified = true;
            var result = _unitOfWork.Repository<FarmerProfile>().Update(farmerProfile);
            // you can use user manager to update the user but it will make 2 calls to the database so i prefer to update the user with the unit of work and then call complete async once to save all changes
            //await _userManager.UpdateAsync(farmerProfile.User);
            var saveresult = await _unitOfWork.CompleteAsync();
            if (!result.IsVerified || saveresult <= 0)
                return ApiResponse<bool>.Fail("فشل في تحديث حالة المزارع حاول مرة اخرى لاحقاَ");

            return ApiResponse<bool>.Ok(true,"تم التحقق من المزارع بنجاح");
        }

        public async Task<ApiResponse<bool>> VerifyTraderAsync(string traderId)
        {
            var traderSpecification = new TraderSpecifications(t => t.TraderId == traderId); 
            var traderProfile = await _unitOfWork.Repository<TraderProfile>().GetByIdWithSpecAsync(traderSpecification);
            if (traderProfile is null)
                return ApiResponse<bool>.Fail("هذا التاجر لا يملك بروفايل ");

            traderProfile.IsVerified = true;
            traderProfile.VerifiedAt = DateTime.UtcNow;
            traderProfile!.User!.IsVerified = true;
            var result = _unitOfWork.Repository<TraderProfile>().Update(traderProfile);
            // you can use user manager to update the user but it will make 2 calls to the database so i prefer to update the user with the unit of work and then call complete async once to save all changes
            //await _userManager.UpdateAsync(traderProfile.User);
            var saveResult = await _unitOfWork.CompleteAsync();
            if (!result.IsVerified || saveResult <= 0)
                return ApiResponse<bool>.Fail("فشل في تحديث حالة التاجر حاول مرة اخرى لاحقاَ");

            return ApiResponse<bool>.Ok(true, "تم التحقق من التاجر بنجاح");
        }

        public  async Task<ApiResponse<IReadOnlyList<BannedUserDto>>> GetBannedUsersAsync()
        {
            var bannedUsers = await _unitOfWork.UserRepository.GetBannedUsersAsync();
            if (!bannedUsers.Any())
            {
                return ApiResponse<IReadOnlyList<BannedUserDto>>.Fail("لا يوجد مستخدمين محظورين حاليا");
            }
            var bannedUsersDtos = bannedUsers.Select(u => new BannedUserDto 
            { 
                Id = u.Id,
                UserName = u.UserName!,
                Email = u.Email!,
                Name = u.Name
            });

            return ApiResponse<IReadOnlyList<BannedUserDto>>.Ok(bannedUsersDtos.ToList(),"تم الحصول علي المستخدمين المحظورين بنجاح");
        }

        public async Task<ApiResponse<string>> ToggleUserStatusAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null)
                return ApiResponse<string>.Fail("هذا المستخدم غير موجود");

            var roles = await _userManager.GetRolesAsync(user);
            if (roles.Contains("Admin"))
                return ApiResponse<string>.Fail("لا تسطيع تغير حالة الحساب الخاص بمسؤول");

            user.IsActive = !user.IsActive;

            if (!user.IsActive)
                await RevokeAllRefreshTokensAsync(userId);

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                return ApiResponse<string>.Fail("فشل في تحديث حالة المستخدم حاول مرة اخرى لاحقاَ");

            var statusMessage = user.IsActive ? "تم تفعيل المستخدم" : "تم حظر المستخدم";
            return ApiResponse<string>.Ok($"تم {statusMessage} بنجاح");
        }

        public async Task<ApiResponse<bool>> DeleteUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null)
                return ApiResponse<bool>.Fail("هذا المستخدم غير موجود");
            var roles = await _userManager.GetRolesAsync(user);
            if (roles.Contains("Admin"))
                return ApiResponse<bool>.Fail("لا تسطيع حذف الحساب الخاص بمسؤول");
            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
                return ApiResponse<bool>.Fail("فشل في حذف المستخدم حاول مرة اخرى لاحقاَ");

            return ApiResponse<bool>.Ok(true, "تم حذف المستخدم بنجاح");
        }

        public async Task<ApiResponse<AdminUserDto>> GetUserByIdAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null)
                return ApiResponse<AdminUserDto>.Fail("هذا المستخدم غير موجود");
            var adminUserDto = _mapper.Map<AdminUserDto>(user);
            adminUserDto.ProfileImageUrl = $"{_baseUrl}{user.ProfileImageUrl}";
            var roles = await _userManager.GetRolesAsync(user);
            adminUserDto.Role = roles.FirstOrDefault() ?? "";
            adminUserDto.TotalProducts = adminUserDto.Role == "Farmer" ? await _unitOfWork.Repository<Product>().CountAsync(p => p.FarmerId == userId) : 0;
            adminUserDto.TotalOrders = adminUserDto.Role == "Trader" ? await _unitOfWork.Repository<Order>().CountAsync(o => o.BuyerId == userId) : 0;
            return ApiResponse<AdminUserDto>.Ok(adminUserDto, "تم الحصول علي المستخدم بنجاح");
        }

        public async Task<ApiResponse<AdminUserDto>> GetAdminByIdAsync(string adminId)
        {
            var user = await _userManager.FindByIdAsync(adminId);
            if (user is null)
                return ApiResponse<AdminUserDto>.Fail("هذا المسؤول غير موجود");

            var roles = await _userManager.GetRolesAsync(user);
            if (!roles.Contains("Admin"))
                return ApiResponse<AdminUserDto>.Fail("هذا المستخدم ليس مسؤول");

            var adminUserDto = _mapper.Map<AdminUserDto>(user);
            adminUserDto.ProfileImageUrl = $"{_baseUrl}{user.ProfileImageUrl}";
            adminUserDto.Role = "Admin";
            return ApiResponse<AdminUserDto>.Ok(adminUserDto, "تم الحصول علي المسؤول بنجاح");
        }


        public async Task<ApiResponse<AdminProfileDto>> GetMyProfileAsAdmin(string adminId)
        {
            var user = await _userManager.FindByIdAsync(adminId);
            if (user is null)
                return ApiResponse<AdminProfileDto>.Fail("هذا المسؤول غير موجود");

            //var roles = await _userManager.GetRolesAsync(user);
            //if (!roles.Contains("Admin"))
            //    return ApiResponse<AdminUserDto>.Fail("هذا المستخدم ليس مسؤول");

            var adminUserDto = _mapper.Map<AdminProfileDto>(user);
            adminUserDto.ProfileImageUrl = $"{_baseUrl}{user.ProfileImageUrl}";
            var addSpecs = new BaseSpecifications<Address>(a => a.UserId == adminId && a.IsDefault);
            var address = await _unitOfWork.Repository<Address>().GetByIdWithSpecAsync(addSpecs);
            if (address is not null)
                adminUserDto.Address = _mapper.Map<AddressDetailsDto>(address);

            return ApiResponse<AdminProfileDto>.Ok(adminUserDto, "تم الحصول علي المسؤول بنجاح");
        }

        public async Task<ApiResponse<AdminProfileDto>> UpdateMyProfileAsAdmin(string adminId, UpdateAdminProfileDto dto)
        {
            var admin = await _userManager.FindByIdAsync(adminId);
            if (admin is null)
                return ApiResponse<AdminProfileDto>.Fail("هذا المستخدم غير موجود");
            admin.Name = string.IsNullOrEmpty(dto.Name) ? admin.Name : dto.Name;
            admin.Email = string.IsNullOrEmpty(dto.Email) ? admin.Email : dto.Email;
            admin.UserName = string.IsNullOrEmpty(dto.UserName) ? admin.UserName : dto.UserName;

            var result = await _userManager.UpdateAsync(admin);
            if (!result.Succeeded)
                return ApiResponse<AdminProfileDto>.Fail("فشل حفظ التعديل حاول لاحقاً");
            return ApiResponse<AdminProfileDto>.Ok(_mapper.Map<AdminProfileDto>(admin), "تم الحصول علي الادمن بنجاح");
        }
        public async Task<ApiResponse<DashboardStatsDto>> GetDashboardStatsAsync()
        {
            var now = DateTime.UtcNow;
            var startOfMonth = new DateTime(now.Year, now.Month, 1);

            var farmers = await _userManager.GetUsersInRoleAsync("Farmer");
            var traders = await _userManager.GetUsersInRoleAsync("Trader");

            var allUsers = await _userManager.Users.ToListAsync();

            var stats = new DashboardStatsDto
            {
                // Users
                TotalUsers = allUsers.Count,
                TotalFarmers = farmers.Count,
                TotalTraders = traders.Count,
                BannedUsers = allUsers.Count(u => !u.IsActive),
                PendingVerifications = farmers.Count(u => !u.IsVerified) + traders.Count(u => !u.IsVerified),
                NewUsersThisMonth = allUsers.Count(u => u.JoinDate >= startOfMonth),

                // Products
                TotalProducts = await _unitOfWork.Repository<Product>().CountAsync(p => true),
                ActiveProducts = await _unitOfWork.Repository<Product>().CountAsync(p => p.Status == ProductStatus.Active),
                SoldOutProducts = await _unitOfWork.Repository<Product>().CountAsync(p => p.Status == ProductStatus.SoldOut),
                UnderReviewProducts = await _unitOfWork.Repository<Product>().CountAsync(p => p.Status == ProductStatus.UnderReview),
                NewProductsThisMonth = await _unitOfWork.Repository<Product>().CountAsync(p => p.CreatedAt >= startOfMonth),
                // Orders
                TotalOrders = await _unitOfWork.Repository<Order>().CountAsync(o => true),
                PendingOrders = await _unitOfWork.Repository<Order>().CountAsync(o => o.Status == OrderStatus.Pending),
                DeliveredOrders = await _unitOfWork.Repository<Order>().CountAsync(o => o.Status == OrderStatus.Delivered),
                CancelledOrders = await _unitOfWork.Repository<Order>().CountAsync(o => o.Status == OrderStatus.Cancelled),
                //TotalRevenue = await _unitOfWork.Repository<Order>().SumAsync(o => o.Status == OrderStatus.Delivered? o.SubTotal : 0),
                //RevenueThisMonth = await _unitOfWork.Repository<Order>().SumAsync(o => o.Status == OrderStatus.Delivered &&o.CreatedAt >= startOfMonth? o.SubTotal : 0),
                NewOrdersThisMonth = await _unitOfWork.Repository<Order>().CountAsync(o => o.CreatedAt >= startOfMonth),

                // Auctions
                TotalAuctions = await _unitOfWork.Repository<Auction>().CountAsync(a => true),
                ActiveAuctions = await _unitOfWork.Repository<Auction>().CountAsync(a => a.Status == AuctionStatus.Active),
                EndedAuctions = await _unitOfWork.Repository<Auction>().CountAsync(a => a.Status == AuctionStatus.Ended),

                // Community
                TotalPosts = 0,// await _unitOfWork.Repository<Post>().CountAsync(p => true),
                PendingReviews = await _unitOfWork.Repository<Review>().CountAsync(r => !r.IsApproved)
            };

            return ApiResponse<DashboardStatsDto>.Ok(stats);
        }

        public async Task<ApiResponse<Pagination<AdminUserDto>>> GetAllUsersAsync(AdminUserFilterDto filter)
        {
            var query = _userManager.Users.AsQueryable();

            if (!string.IsNullOrEmpty(filter.SearchTerm))
                query = query.Where(u => u.Name.Contains(filter.SearchTerm) ||
                                         u.Email!.Contains(filter.SearchTerm));

            if (filter.IsActive.HasValue)
                query = query.Where(u => u.IsActive == filter.IsActive);

            if (filter.IsVerified.HasValue)
                query = query.Where(u => u.IsVerified == filter.IsVerified);

            if (filter.JoinedFrom.HasValue)
                query = query.Where(u => u.JoinDate >= filter.JoinedFrom);

            if (filter.JoinedTo.HasValue)
                query = query.Where(u => u.JoinDate <= filter.JoinedTo);

            query = filter.SortBy switch
            {
                "name" => filter.SortDescending ? query.OrderByDescending(u => u.Name) : query.OrderBy(u => u.Name),
                "joinDate" => filter.SortDescending ? query.OrderByDescending(u => u.JoinDate) : query.OrderBy(u => u.JoinDate),
                _ => query.OrderByDescending(u => u.JoinDate)
            };

            var totalCount = await query.CountAsync();
            var users = await query.Skip((filter.PageNumber - 1) * filter.PageSize).Take(filter.PageSize).ToListAsync();

            if (totalCount == 0)
                return ApiResponse<Pagination<AdminUserDto>>.Fail("لايوجد مستخدمين حالين مستوفين هذه الشروط");
            // Filter by role if specified
            var result = new List<AdminUserDto>();
            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                if (!string.IsNullOrEmpty(filter.Role) && !roles.Contains(filter.Role)) continue;

                var role = roles.FirstOrDefault() ?? "Unknown";
                //var farmerProfile = role == "Farmer" ? await _unitOfWork.Repository<FarmerProfile>().GetByIdAsync(user.Id) : null;
                //var traderProfile = role == "Trader" ? await _unitOfWork.Repository<TraderProfile>().GetByIdAsync(user.Id) : null;
                var totalOrders = role == "Trader" ? await _unitOfWork.Repository<Order>().CountAsync(o => o.BuyerId == user.Id) : 0;
                var totalProducts = role == "Farmer"? await _unitOfWork.Repository<Product>().CountAsync(p => p.FarmerId == user.Id) : 0;
                //var totalSpent = await _unitOfWork.Repository<Order>().SumAsync(o => o.BuyerId == user.Id && o.Status == OrderStatus.Delivered? o.SubTotal : 0);

                result.Add(new AdminUserDto
                {
                    Id = user.Id,
                    Name = user.Name,
                    Email = user.Email!,
                    PhoneNumber = user.PhoneNumber??"",
                    Role = role,
                    IsActive = user.IsActive,
                    IsVerified = user.IsVerified,
                    JoinDate = user.JoinDate,
                    ProfileImageUrl = $"{_baseUrl}{user.ProfileImageUrl}",
                    //FarmName = farmerProfile?.FarmName,
                    //BusinessName = traderProfile?.BusinessName,
                    TotalOrders = totalOrders,
                    TotalProducts = totalProducts,
                });
            }
            return ApiResponse<Pagination<AdminUserDto>>.Ok(new Pagination<AdminUserDto>(filter.PageNumber, filter.PageSize, totalCount, result),"تم الحصول علي المستخدمين بنجاح");    
        }

        #region MyRegion
        //public async Task<ApiResponse<string>> DeleteUserAsync(string userId)
        //{
        //    var user = await _userManager.FindByIdAsync(userId);
        //    if (user is null)
        //        return ApiResponse<string>.Fail("User not found");

        //    var roles = await _userManager.GetRolesAsync(user);
        //    if (roles.Contains("Admin"))
        //        return ApiResponse<string>.Fail("Cannot delete an admin account");

        //    // Soft delete — just deactivate instead of hard delete
        //    // Hard delete would break order history, reviews, etc.
        //    user.IsActive = false;
        //    user.Email = $"deleted_{user.Id}@deleted.com";
        //    user.UserName = $"deleted_{user.Id}";

        //    var result = await _userManager.UpdateAsync(user);
        //    if (!result.Succeeded)
        //        return ApiResponse<string>.Fail("Failed to delete user");

        //    return ApiResponse<string>.Ok("User deleted successfully");
        //}
        //        public async Task<ApiResponse<CategoryDto>> CreateCategoryAsync(
        //        CreateCategoryDto dto)
        //        {
        //            // Check parent exists if provided
        //            if (dto.ParentCategoryId.HasValue)
        //            {
        //                var parent = await _unitOfWork.Categories
        //                                              .GetByIdAsync(dto.ParentCategoryId.Value);
        //                if (parent is null)
        //                    return ApiResponse<CategoryDto>.Fail("Parent category not found");
        //            }

        //            var category = new Category
        //            {
        //                Name = dto.Name,
        //                NameAr = dto.NameAr,
        //                ParentCategoryId = dto.ParentCategoryId
        //            };

        //            await _unitOfWork.Categories.AddAsync(category);
        //            await _unitOfWork.SaveChangesAsync();

        //            return ApiResponse<CategoryDto>.Ok(MapToCategoryDto(category));
        //        }

        //        public async Task<ApiResponse<string>> DeleteCategoryAsync(int id)
        //        {
        //            var category = await _unitOfWork.Categories.GetByIdAsync(id);
        //            if (category is null)
        //                return ApiResponse<CategoryDto>.Fail("Category not found");

        //            // Check if any products use this category
        //            var hasProducts = await _unitOfWork.Products
        //                                               .AnyAsync(p => p.CategoryId == id);
        //            if (hasProducts)
        //                return ApiResponse<string>.Fail(
        //                    "Cannot delete category that has products. " +
        //                    "Move products to another category first.");

        //            // Check if has subcategories
        //            var hasSubCategories = await _unitOfWork.Categories
        //                                                    .AnyAsync(c => c.ParentCategoryId == id);
        //            if (hasSubCategories)
        //                return ApiResponse<string>.Fail(
        //                    "Cannot delete category that has subcategories. " +
        //                    "Delete subcategories first.");

        //            _unitOfWork.Categories.Delete(category);
        //            await _unitOfWork.SaveChangesAsync();

        //            return ApiResponse<string>.Ok("Category deleted successfully");
        //        }

        //        // ── Community Moderation ─────────────────────────────
        //        public async Task<ApiResponse<string>> DeletePostAsync(int postId)
        //        {
        //            var post = await _unitOfWork.Posts.GetByIdAsync(postId);
        //            if (post is null)
        //                return ApiResponse<string>.Fail("Post not found");

        //            post.IsDeleted = true;  // soft delete
        //            await _unitOfWork.SaveChangesAsync();

        //            return ApiResponse<string>.Ok("Post removed successfully");
        //        }

        //        public async Task<ApiResponse<string>> DeleteCommentAsync(int commentId)
        //        {
        //            var comment = await _unitOfWork.PostComments.GetByIdAsync(commentId);
        //            if (comment is null)
        //                return ApiResponse<string>.Fail("Comment not found");

        //            comment.IsDeleted = true;
        //            await _unitOfWork.SaveChangesAsync();

        //            return ApiResponse<string>.Ok("Comment removed successfully");
        //        }

        //        // ── Auction Management ───────────────────────────────
        //        public async Task<ApiResponse<string>> CancelAuctionAsync(
        //            int auctionId, string reason)
        //        {
        //            var auction = await _unitOfWork.Auctions.GetByIdAsync(auctionId);
        //            if (auction is null)
        //                return ApiResponse<string>.Fail("Auction not found");

        //            if (auction.Status == AuctionStatus.Ended)
        //                return ApiResponse<string>.Fail("Cannot cancel an ended auction");

        //            auction.Status = AuctionStatus.Cancelled;

        //            // Release the product
        //            var product = await _unitOfWork.Products.GetByIdAsync(auction.ProductId);
        //            if (product is not null)
        //                product.Status = ProductStatus.Active;

        //            await _unitOfWork.SaveChangesAsync();

        //            return ApiResponse<string>.Ok($"Auction cancelled. Reason: {reason}");
        //        }
        //}
        #endregion
        private async Task RevokeAllRefreshTokensAsync(string userId)
        {
            var refreshTokenSpecObject = new BaseSpecifications<RefreshToken>(r => r.UserId == userId && r.IsActive);
            var token = await _unitOfWork.Repository<RefreshToken>().GetByIdWithSpecAsync(refreshTokenSpecObject);
            if (token is not null)
            {
                token.RevokedOn = DateTime.UtcNow;

                await _unitOfWork.CompleteAsync();
            }
            
        }

    }
}
