using Microsoft.Extensions.Configuration;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using T3awuny.Application.Common;
using T3awuny.Application.Contracts;
using T3awuny.Core;
using T3awuny.Core.Entities;
using T3awuny.Core.Entities.BasketModule;
using T3awuny.Core.Entities.Enums;
using T3awuny.Core.Entities.OrderAggregate;
using T3awuny.Core.Repository.Contracts;
using T3awuny.Core.Specifications;
using Product = T3awuny.Core.Entities.Product;

namespace T3awuny.Application.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBasketRepository _basketRepository;
        private readonly IConfiguration _configuration;

        public PaymentService(IBasketRepository basketRepository, IUnitOfWork unitOfWork, IConfiguration configuration)
        {
            _basketRepository = basketRepository;
            _unitOfWork = unitOfWork;
            _configuration = configuration;
        }

        public async Task<ApiResponse<CustomerBasket>> CreateOrUpdatePaymentIntentAsync(string basketId)
        {
            //get customer basket
            var basket = await _basketRepository.GetBasketAsync(basketId);
            if (basket is null)
                return ApiResponse<CustomerBasket>.Fail("هذه السلة غير متاحة");

            if(!basket.DeliveryMethodId.HasValue)
                //deliveryMethod = await _unitOfWork.Repository<DeliveryMethod>().GetByIdAsync(4); // no need to get from DB its cost is know to 0
                basket.ShippingPrice = 0;
            else
            {
                var deliveryMethod = await _unitOfWork.Repository<DeliveryMethod>().GetByIdAsync(basket.DeliveryMethodId.Value);
                if(deliveryMethod is null)
                    return ApiResponse<CustomerBasket>.Fail("طريقة التوصيل هذه غير متاحة");
                basket.ShippingPrice = deliveryMethod.Cost;
            }

            if(!basket.Items.Any())
                return ApiResponse<CustomerBasket>.Fail("هذه السلة فارغة اضف بعض المنتجات لمزارع واحد");

            var productRepo = _unitOfWork.Repository<Product>();
            foreach (var item in basket.Items)
            {
                var product = await productRepo.GetByIdAsync(item.ProductId);
                if (product is null)
                {
                    basket.Items.Remove(item);
                    continue;
                }

                if(item.Price != product.UnitPrice)
                    item.Price = product.UnitPrice;


            }

            StripeConfiguration.ApiKey = _configuration["StripeSettings:SecretKey"];

            PaymentIntentService paymentIntentService = new PaymentIntentService();

            PaymentIntent paymentIntent;

            if(string.IsNullOrEmpty(basket.PaymentIntentId)) // Create new payment intent 
            {
                var createOptions = new PaymentIntentCreateOptions()
                {
                    Amount = (long)basket.Items.Sum(item => item.Price * 100 /*/ 53*/ * item.Quantity) + (long)basket.ShippingPrice,
                    Currency = "usd",
                    PaymentMethodTypes = new List<string>() { "card" }//, "paypal"
                };
                paymentIntent = await paymentIntentService.CreateAsync(createOptions);

                basket.PaymentIntentId = paymentIntent.Id;
                basket.ClientSecret = paymentIntent.ClientSecret;
            }
            else // update the amount o the current payment intent
            {
                var updateOptions = new PaymentIntentUpdateOptions()
                {
                    Amount = (long)basket.Items.Sum(item => item.Price * 100 /*/ 53*/ * item.Quantity) + (long)basket.ShippingPrice
                };
                paymentIntent = await paymentIntentService.UpdateAsync(basket.PaymentIntentId,updateOptions);
            }

            await _basketRepository.CreateOrUpdateBasketAsync(basket);
            return ApiResponse<CustomerBasket>.Ok(basket,"تم إضافة نية الدفع بنجاح");
        }

        public async Task<ApiResponse<Payment>> GetPaymentByOrderAsync(int orderId)
        {
            var paymentSpecs = new BaseSpecifications<Payment>(p => p.OrderId == orderId);
            var payment = await _unitOfWork.Repository<Payment>().GetByIdWithSpecAsync(paymentSpecs);
            if (payment is null)
                return ApiResponse<Payment>.Fail("بيانات الدفع غير متوفرة");

            return ApiResponse<Payment>.Ok(payment,"تم الحصول علي بيانات عملية الدفع بنجاح");
        }

        public async Task<ApiResponse<IReadOnlyList<Payment>>> GetPaymentsAsync()
        {
            var payments = await _unitOfWork.Repository<Payment>().GetAllAsync();

            if (!payments.Any())
                return ApiResponse<IReadOnlyList<Payment>>.Fail("لا يوجد بيانات دفع لعرضها");        

            return ApiResponse<IReadOnlyList<Payment>>.Ok(payments,"تم الحصول علي بيانات الدفع بنجاح");
        }

        public async Task<ApiResponse<string>> UpdatePaymentStatusAsync(int orderId, PaymentStatus status)
        {
            var order = await _unitOfWork.Repository<Order>().GetByIdAsync(orderId);
            if (order is null)
                return ApiResponse<string>.Fail("هذا الطلب غير متاح");

            var paymentSpecs = new BaseSpecifications<Payment>(p => p.OrderId == orderId);
            var payment = await _unitOfWork.Repository<Payment>().GetByIdWithSpecAsync(paymentSpecs);
            if (payment is null)
                return ApiResponse<string>.Fail("بيانات الدفع لهذا الطلب غير متوفرة");

            payment.Status = status;
            order.PaymentStatus = status;
            if(await _unitOfWork.CompleteAsync() <= 0)
                return ApiResponse<string>.Fail("حدثت مشكلة أثناء حفظ البيانات حاول لاحقاً");

            return ApiResponse<string>.Ok(payment.Id.ToString(), "تم تحديث حالة الدفع لهذا الطلب بنجاح");
        }

        public async Task<bool> UpdatePaymentStatusToFailOrSuccess(string paymentIntentId, bool isSuccess)
        {
            var paymentSpec = new BaseSpecifications<Payment>(p => p.PaymentIntentId == paymentIntentId);
            var payment = await _unitOfWork.Repository<Payment>().GetByIdWithSpecAsync(paymentSpec);
            if(payment is null)
                return false;

            var orderSpec = new BaseSpecifications<Order>(o => o.PaymentIntentId == paymentIntentId);
            var order = await _unitOfWork.Repository<Order>().GetByIdWithSpecAsync(orderSpec);
            if (order is null)
                return false;
            if (isSuccess)
            {
                payment.Status = PaymentStatus.Paid;
                order.PaymentStatus = PaymentStatus.Paid;
            }
            else
            {
                payment.Status = PaymentStatus.Failed;
                order.PaymentStatus = PaymentStatus.Failed;
            }

            if( await _unitOfWork.CompleteAsync() <= 0)
                return false;
            return true;

        }

    }
}
