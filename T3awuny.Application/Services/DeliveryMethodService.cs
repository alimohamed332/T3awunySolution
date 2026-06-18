using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using T3awuny.Application.Common;
using T3awuny.Application.Contracts;
using T3awuny.Application.DTOs.DeliveryMethods;
using T3awuny.Core;
using T3awuny.Core.Entities.OrderAggregate;

namespace T3awuny.Application.Services
{
    public class DeliveryMethodService : IDeliveryMethodService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public DeliveryMethodService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ApiResponse<IReadOnlyList<DeliveryMethodResponseDto>>> GetAll()
        {
            var deliveryMethods = await _unitOfWork.Repository<DeliveryMethod>().GetAllAsync();
            var deliveryMethodsDtos = deliveryMethods.Select(d => _mapper.Map<DeliveryMethodResponseDto>(d)).ToList();
            if(!deliveryMethodsDtos.Any())
            return ApiResponse<IReadOnlyList<DeliveryMethodResponseDto>>.Ok(deliveryMethodsDtos, "لا يوجد طرق توصيل بعد");

            return ApiResponse<IReadOnlyList<DeliveryMethodResponseDto>>.Ok(deliveryMethodsDtos, "تم الحصول علي طرق التوصيل بنجاح");
        }

        public async Task<ApiResponse<DeliveryMethodResponseDto>> GetDeliveryMethod(int id)
        {
            var deliveryMethod = await _unitOfWork.Repository<DeliveryMethod>().GetByIdAsync(id);
            if (deliveryMethod is null)
                return ApiResponse<DeliveryMethodResponseDto>.Fail("طريقة التوصيل هذه غير موجودة");

            return ApiResponse<DeliveryMethodResponseDto>.Ok(_mapper.Map<DeliveryMethodResponseDto>(deliveryMethod),"تم الحصول علي طريقة التوصيل بنجاح");
        }

        public async Task<ApiResponse<DeliveryMethodResponseDto>> CreateDeliveryMethod(CreateDeliveryMethodDto dto)
        {
            var deliveryMetod = _mapper.Map<DeliveryMethod>(dto);
            await _unitOfWork.Repository<DeliveryMethod>().AddAsync(deliveryMetod);
            if (await _unitOfWork.CompleteAsync() <= 0)
                return ApiResponse<DeliveryMethodResponseDto>.Fail("فشل إضافة طريقة التوصيل حاول لاحقاً");

            return ApiResponse<DeliveryMethodResponseDto>.Ok(_mapper.Map<DeliveryMethodResponseDto>(deliveryMetod), "تمت الأضافة بنجاح");
        }

        public async Task<ApiResponse<bool>> DeleteDeliveryMethod(int deliveryMethodId)
        {
            var deliveryMethod = await _unitOfWork.Repository<DeliveryMethod>().GetByIdAsync(deliveryMethodId);
            if (deliveryMethod is null)
                return ApiResponse<bool>.Fail("طريقة التوصيل هذه غير موجودة");

            _unitOfWork.Repository<DeliveryMethod>().Delete(deliveryMethod);
            if(await _unitOfWork.CompleteAsync() <= 0)
                return ApiResponse<bool>.Fail("حدث خطأ أثناء الحذف حاول لاحقاً");

            return ApiResponse<bool>.Ok(true, "تم حذف طريقة التوصيل بنجاح");
        }
        public async Task<ApiResponse<DeliveryMethodResponseDto>> UpdateDeliveryMethod(int deliveryMethodId, UpdateDeliveryMethodDto dto)
        {
            var deliveryMethod = await _unitOfWork.Repository<DeliveryMethod>().GetByIdAsync(deliveryMethodId);
            if (deliveryMethod is null)
                return ApiResponse<DeliveryMethodResponseDto>.Fail("طريقة التوصيل هذه غير موجودة");

            deliveryMethod.ShortName = string.IsNullOrEmpty(dto.ShortName) ? deliveryMethod.ShortName : dto.ShortName;
            deliveryMethod.Description = string.IsNullOrEmpty(dto.Description) ? deliveryMethod.Description : dto.Description;
            deliveryMethod.DeliveryTime = string.IsNullOrEmpty(dto.DeliveryTime) ? deliveryMethod.DeliveryTime : dto.DeliveryTime;
            deliveryMethod.Cost = dto.Cost.HasValue ? dto.Cost.Value : deliveryMethod.Cost;

            _unitOfWork.Repository<DeliveryMethod>().Update(deliveryMethod);
            if (await _unitOfWork.CompleteAsync() <= 0)
                return ApiResponse<DeliveryMethodResponseDto>.Fail("حدث خطأ أثناء حفظ التعديل حاول لاحقاً");

            return ApiResponse<DeliveryMethodResponseDto>.Ok(_mapper.Map<DeliveryMethodResponseDto>(deliveryMethod), "تم تعديل طريقة التوصيل بنجاح");
        }
    }
}
