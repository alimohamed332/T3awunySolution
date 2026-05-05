using AutoMapper;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using T3awuny.Application.Common;
using T3awuny.Application.Contracts;
using T3awuny.Application.DTOs.Logistics;
using T3awuny.Core;
using T3awuny.Core.Entities.Enums;
using T3awuny.Core.Entities.OrderAggregate;
using T3awuny.Core.Entities.UserModule;
using T3awuny.Core.Specifications;

namespace T3awuny.Application.Services
{
    public class LogisticsService : ILogisticsService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;
        public LogisticsService(IUnitOfWork unitOfWork, IMapper mapper, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userManager = userManager;
        }

        public async Task<ApiResponse<LogisticsResponseDto>> GetByOrderIdAsync(string userId, int orderId)
        {
            var order = await _unitOfWork.Repository<Order>().GetByIdAsync(orderId);
            if (order is null)
                return ApiResponse<LogisticsResponseDto>.Fail("هذا الطلب غير موجود");

            var user = await _userManager.FindByIdAsync(userId);
            if (user is null)
                return ApiResponse<LogisticsResponseDto>.Fail("هذا المستخدم غير موجود");

            var roles = await _userManager.GetRolesAsync(user);

            if (order.BuyerId != userId || order.FarmerId != userId || !roles.Contains("Admin"))
                return ApiResponse<LogisticsResponseDto>.Fail("لا يمكنك الاطلاع علي تفاصيل توصيل طلب لا يخصك");

            var logisticsSpec = new BaseSpecifications<Logistics>(lo => lo.OrderId == orderId);
            var logistics = await _unitOfWork.Repository<Logistics>().GetByIdWithSpecAsync(logisticsSpec);

            if (logistics is null)
                return ApiResponse<LogisticsResponseDto>.Fail("لا يوجد بيانات توصيل تخص هذا الطلب");

            var pickUpAdd = await _unitOfWork.Repository<Address>().GetByIdAsync(logistics.PickupAddressId);
            var deliveryAdd = await _unitOfWork.Repository<Address>().GetByIdAsync(logistics.DeliveryAddressId);


            var logisticsDto = _mapper.Map<LogisticsResponseDto>(logistics);
            if(pickUpAdd is not null && deliveryAdd is not null)
            {
                logisticsDto.PickupAddress = _mapper.Map<OrderAddress>(pickUpAdd);
                logisticsDto.DeliveryAddress = _mapper.Map<OrderAddress>(deliveryAdd);
            }

            return ApiResponse<LogisticsResponseDto>.Ok(logisticsDto, "تم الحصول علي بيانات التوصيل الخاصة بالطلب بنجاح");
        }

        public async Task<ApiResponse<string>> UpdateLogisticsAsync(int orderId, UpdateLogisticsDto dto)
        {

            var logisticSpec = new BaseSpecifications<Logistics>(lo => lo.OrderId == orderId);
            var logistics = await _unitOfWork.Repository<Logistics>().GetByIdWithSpecAsync(logisticSpec);
            if (logistics is null)
                return ApiResponse<string>.Fail("بيانات توصيل هذاالطلب غير متاحة");
            // another way for update 
            if(!string.IsNullOrEmpty(dto.Notes))
                logistics.Notes = dto.Notes;
            if(!string.IsNullOrEmpty(dto.DriverName))
                logistics.DriverName = dto.DriverName;
            if(!string.IsNullOrEmpty(dto.DriverPhone))
                logistics.DriverPhone = dto.DriverPhone;
            if(dto.ActualDelivery.HasValue)
                logistics.ActualDelivery = dto.ActualDelivery.Value;
            if(dto.EstimatedDelivery.HasValue)
                logistics.EstimatedDelivery = dto.EstimatedDelivery.Value;
            if(dto.DeliveryAddressId.HasValue)
                logistics.DeliveryAddressId = dto.DeliveryAddressId.Value;
            if(dto.PickupAddressId.HasValue)
                logistics.PickupAddressId = dto.PickupAddressId.Value;

            if (await _unitOfWork.CompleteAsync() <= 0)
                return ApiResponse<string>.Fail("حدث خطأ أثناء حفظ البيانات حاول لاحقاً");
            return ApiResponse<string>.Ok(string.Empty,"تم تحديث البيانات بنجاح");
        }

        public async Task<ApiResponse<string>> UpdateStatusAsync(int orderId, LogisticsStatus status) // admin only call it
        {
            var logisticSpec = new BaseSpecifications<Logistics>(lo => lo.OrderId == orderId);
            var logistics = await _unitOfWork.Repository<Logistics>().GetByIdWithSpecAsync(logisticSpec);
            if (logistics is null)
                return ApiResponse<string>.Fail("بيانات توصيل هذاالطلب غير متاحة");
            
            logistics.Status = status;
            if(await _unitOfWork.CompleteAsync() <= 0)
                return ApiResponse<string>.Fail("حدث خطأ أثناء تسجيل البيانات حاول مرة اخري لاحقاً");

            return ApiResponse<string>.Ok(string.Empty, "تم تحديث بيانات التوصيل بنجاح");

        }
    }
}
