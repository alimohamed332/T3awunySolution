using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using T3awuny.Application.Common;
using T3awuny.Application.DTOs.DeliveryMethods;

namespace T3awuny.Application.Contracts
{
    public interface IDeliveryMethodService
    {
        Task<ApiResponse<IReadOnlyList<DeliveryMethodResponseDto>>> GetAll();
        Task<ApiResponse<DeliveryMethodResponseDto>> GetDeliveryMethod(int id);
        Task<ApiResponse<DeliveryMethodResponseDto>> CreateDeliveryMethod(CreateDeliveryMethodDto dto);
        Task<ApiResponse<DeliveryMethodResponseDto>> UpdateDeliveryMethod(int deliveryMethodId,UpdateDeliveryMethodDto dto);
        Task<ApiResponse<bool>> DeleteDeliveryMethod(int deliveryMethodId);
    }
}
