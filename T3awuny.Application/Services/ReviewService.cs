using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using T3awuny.Application.Common;
using T3awuny.Application.Contracts;
using T3awuny.Application.DTOs.Review;
using T3awuny.Core;
using T3awuny.Core.Entities.Enums;
using T3awuny.Core.Entities.OrderAggregate;
using T3awuny.Core.Entities.ReviewModule;
using T3awuny.Core.Entities.UserModule;
using T3awuny.Core.Specifications;
using T3awuny.Core.Specifications.ReviewSpecs;

namespace T3awuny.Application.Services
{
    public class ReviewService : IReviewService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly string _baseUrl;

        public ReviewService(IUnitOfWork unitOfWork, IMapper mapper, UserManager<ApplicationUser> userManager, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userManager = userManager;
            _baseUrl = configuration["App:ApplicationUrl"] ??"";
        }

        public async Task<ApiResponse<ReviewResponseDto>> CreateReviewAsync(string reviewerId, CreateReviewDto dto)
        {
            var reviewer = await _userManager.FindByIdAsync(reviewerId);
            if(reviewer is null)
                return ApiResponse<ReviewResponseDto>.Fail("هذا المستخدم غير موجود");
            #region كل ده تحقق مرتيط بالطلب نفسه هنلغيه عشان علي نصر مش عايز يبعت ال orderId 
            //// 1. Validate order exists and belongs to reviewer
            var order = await _unitOfWork.Repository<Order>().GetByIdAsync(dto.OrderId);
            if (order is null || order.BuyerId != reviewerId || order.FarmerId != dto.TargetUserId)
                return ApiResponse<ReviewResponseDto>.Fail("الطلب غير موجود او لا يخص هذا المستخدم");

            //// 2. Order must be delivered
            //if (order.Status != OrderStatus.Delivered)
            //    return ApiResponse<ReviewResponseDto>.Fail("You can only review after the order is delivered");

            //// 3. Check no existing review for this order
            //var existingReview = await _unitOfWork.Reviews.GetByReviewerAndOrderAsync(reviewerId, dto.OrderId);
            //if (existingReview is not null)
            //    return ApiResponse<ReviewResponseDto>.Fail("You have already reviewed this order");

            //////  Validate rating => or [Range(1,5, ErrorMessage = "التقيم يجب أن يكون بين 1 و 5")] in DTO
            ////if (dto.Rating < 1 || dto.Rating > 5)
            ////    return ApiResponse<ReviewResponseDto>.Fail("Rating must be between 1 and 5");

            //// Validate farmer matches the order
            //var orderItem = await _unitOfWork.OrderItems
            //                                 .GetFirstByOrderIdAsync(dto.OrderId);
            //if (orderItem?.Product.FarmerId != dto.TargetFarmerId)
            //    return ApiResponse<ReviewResponseDto>.Fail("Invalid farmer for this order"); 
            #endregion
            //check if the target user has any completed orders with the reviewer => either as buyer or farmer both can review each other
            var orderSpec = new BaseSpecifications<Order>(o => (o.BuyerId == reviewerId || o.BuyerId == dto.TargetUserId) &&
                                                 (o.FarmerId == dto.TargetUserId || o.FarmerId == reviewerId)&&
                                                 o.Status == OrderStatus.Delivered && o.CreatedAt.AddDays(30) >= DateTime.UtcNow);
            //ده كده هيرجع الطلبات اللي بين الاتنين دول اللي تم توصيلها خلاص ومعداش عليها شهر من تاريخ الإنشاء يعني لسه في فترة السماح للتقييم
            var completedOrders = await _unitOfWork.Repository<Order>().GetAllWithSpecAsync(orderSpec);

            if(!completedOrders.Any())
                return ApiResponse<ReviewResponseDto>.Fail("لا يوجد اي طلبات مكتملة مع هذا المستخدم اخر شهر لذلك لا يمكنك تقييمه");

            // Create review
            var review = new Review
            {
                ReviewerId = reviewerId,
                TargetUserId = dto.TargetUserId,
                OrderId = dto.OrderId,
                Rating = dto.Rating,
                Comment = dto.Comment,
                IsApproved = false,          // pending admin approval
                CreatedAt = DateTime.UtcNow
            };
            //i need to get the reviewer add recviewspecification class
            await _unitOfWork.Repository<Review>().AddAsync(review);
            if (await _unitOfWork.CompleteAsync() <=0 )
                return ApiResponse<ReviewResponseDto>.Fail("فشل في تقديم التقييم حاول لاحقاً");

            var reviewResponse = _mapper.Map<ReviewResponseDto>(review);
            reviewResponse.ReviewerName = reviewer.Name;
            reviewResponse.ReviewerImageUrl = $"{_baseUrl}{reviewer.ProfileImageUrl}";
            return ApiResponse<ReviewResponseDto>.Ok(reviewResponse, "تم تقديم التقييم وهو قيد الموافقة");
        }

        public async Task<ApiResponse<IReadOnlyList<ReviewResponseDto>>> GetUserReviewsAsync(string userId)
        {
            var reviewsSpec = new ReviewSpecifications(r => r.TargetUserId == userId && r.IsApproved);
            var reviews = await _unitOfWork.Repository<Review>().GetAllWithSpecAsync(reviewsSpec);

            if(!reviews.Any())
                return ApiResponse<IReadOnlyList<ReviewResponseDto>>.Ok(new List<ReviewResponseDto>(),"لا يوجد تقييمات تم اعتمادها من المشرف علي هذا المستخدم حتى الآن");
            var reviewDtos =new List<ReviewResponseDto>();
            foreach (var review in reviews)
            {
                var reviewDto = _mapper.Map<ReviewResponseDto>(review);
                reviewDto.ReviewerImageUrl = $"{_baseUrl}{reviewDto.ReviewerImageUrl}";
                reviewDtos.Add(reviewDto);
            }
            return ApiResponse<IReadOnlyList<ReviewResponseDto>>.Ok(reviewDtos, "تم الحصول على التقييمات بنجاح");
        }

        public async Task<ApiResponse<UserRatingSummaryDto>> GetUserRatingSummaryAsync(string userId)
        {
            var reviewsSpec = new BaseSpecifications<Review>(r => r.TargetUserId == userId && r.IsApproved);
            var reviews = await _unitOfWork.Repository<Review>().GetAllWithSpecAsync(reviewsSpec);
            var reviewList = reviews.ToList();

            if (!reviewList.Any())
                return ApiResponse<UserRatingSummaryDto>.Ok(new UserRatingSummaryDto
                {
                    UserId = userId,
                    AverageRating = 0,
                    TotalReviews = 0
                },"لا يوجد تقييمات معتمدة لهذا المستخدم حتى الآن");

            return ApiResponse<UserRatingSummaryDto>.Ok(new UserRatingSummaryDto
            {
                UserId = userId,
                AverageRating = Math.Round(reviewList.Average(r => r.Rating), 1),
                TotalReviews = reviewList.Count,
                FiveStars = reviewList.Count(r => r.Rating == 5),
                FourStars = reviewList.Count(r => r.Rating == 4),
                ThreeStars = reviewList.Count(r => r.Rating == 3),
                TwoStars = reviewList.Count(r => r.Rating == 2),
                OneStar = reviewList.Count(r => r.Rating == 1)
            },"تم الحصول على ملخص التقييمات بنجاح");
        }

        public async Task<ApiResponse<string>> ApproveReviewAsync(int reviewId)
        {
            var review = await _unitOfWork.Repository<Review>().GetByIdAsync(reviewId);
            if (review is null)
                return ApiResponse<string>.Fail("التقييم غير موجود");

            review.IsApproved = true;
            if (await _unitOfWork.CompleteAsync() <= 0)
                return ApiResponse<string>.Fail("فشل في اعتماد التقييم حاول لاحقاً");

            return ApiResponse<string>.Ok(reviewId.ToString(),"تم اعتماد التقييم بنجاح");
        }

        public async Task<ApiResponse<string>> DeleteReviewAsync(int reviewId)
        {
            var review =  await _unitOfWork.Repository<Review>().GetByIdAsync(reviewId);
            if(review is null)
                return ApiResponse<string>.Fail("التقييم غير موجود");

            _unitOfWork.Repository<Review>().Delete(review);
            if (await _unitOfWork.CompleteAsync() <= 0)
                return ApiResponse<string>.Fail("فشل في حذف التقييم حاول لاحقاً");

            return ApiResponse<string>.Ok(reviewId.ToString(),"تم حذف التقييم بنجاح");
        }

        public async Task<ApiResponse<IReadOnlyList<ReviewResponseDto>>> GetPendingReviewsAsync()
        {
            var reviewsSpec = new ReviewSpecifications(r => !r.IsApproved);
            var reviews =  await _unitOfWork.Repository<Review>().GetAllWithSpecAsync(reviewsSpec);//reviewer

            if(!reviews.Any())
                return ApiResponse<IReadOnlyList<ReviewResponseDto>>.Ok(new List<ReviewResponseDto>(),"لا يوجد تقييمات قيد الانتظار");
            var reviewDtos =new List<ReviewResponseDto>();
            foreach (var review in reviews)
            {
                var reviewDto = _mapper.Map<ReviewResponseDto>(review);
                reviewDto.ReviewerImageUrl = $"{_baseUrl}{reviewDto.ReviewerImageUrl}";
                reviewDto.TargetImageUrl = $"{_baseUrl}{reviewDto.TargetImageUrl}";
                reviewDtos.Add(reviewDto);
            }
            return ApiResponse<IReadOnlyList<ReviewResponseDto>>.Ok(reviewDtos, "تم الحصول على التقييمات قيد الانتظار بنجاح");
        }
    }
}
