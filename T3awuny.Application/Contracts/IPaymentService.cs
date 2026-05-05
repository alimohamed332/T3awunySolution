using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using T3awuny.Application.Common;
using T3awuny.Core.Entities.BasketModule;
using T3awuny.Core.Entities.Enums;
using T3awuny.Core.Entities.OrderAggregate;

namespace T3awuny.Application.Contracts
{
    public interface IPaymentService
    {
        Task<ApiResponse<CustomerBasket>> CreateOrUpdatePaymentIntentAsync(string basketId);
        Task<bool> UpdatePaymentStatusToFailOrSuccess(string paymentIntentId, bool isSuccess);
        Task<ApiResponse<IReadOnlyList<Payment>>> GetPaymentsAsync();
        Task<ApiResponse<Payment>> GetPaymentByOrderAsync(int orderId);
        Task<ApiResponse<string>> UpdatePaymentStatusAsync(int orderId, PaymentStatus status);
    }
}
