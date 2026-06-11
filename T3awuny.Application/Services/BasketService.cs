using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using T3awuny.Application.Common;
using T3awuny.Application.Contracts;
using T3awuny.Application.DTOs.Basket;
using T3awuny.Core.Entities.BasketModule;
using T3awuny.Core.Repository.Contracts;

namespace T3awuny.Application.Services
{
    public class BasketService : IBaskeetService
    {
        private readonly IBasketRepository _basketRepo;

        public BasketService(IBasketRepository basketRepository)
        {
            _basketRepo = basketRepository;
        }

        public async Task<ApiResponse<CustomerBasket>> CreateOrUpdateBasketAsync(CreateBasketDto basket)
        {
            var createbasket = new CustomerBasket();
            createbasket.Id = basket.Id ?? "";
            createbasket.Items = basket.Items;
            createbasket.DeliveryMethodId = basket.DeliveryMethodId;
            CustomerBasket? basketFromRepo;
            try
            {
                basketFromRepo = await _basketRepo.CreateOrUpdateBasketAsync(createbasket);
            }
            catch
            {
                return ApiResponse<CustomerBasket>.Fail("مشكلة في التواصل مع Redis");
            }
            if (basketFromRepo is null)
                return ApiResponse<CustomerBasket>.Fail("هذه العربة غير موجودة");
            return ApiResponse<CustomerBasket>.Ok(basketFromRepo,"تم اضافة العربة بنجاح");
        }

        public async Task<ApiResponse<bool>> DeleteBasketAsync(string basketId)
        {
            bool basketDeleted;
            try
            {
                 basketDeleted = await _basketRepo.DeleteBasketAsync(basketId);
            }
            catch (Exception) 
            {
                return ApiResponse<bool>.Fail("مشكلة في التواصل مع Redis");
            }
            
            if(!basketDeleted)
                return ApiResponse<bool>.Fail("هذه العربة غير موجودة او فشل حذفها حاول مرة اخري لاحقاً");
            return ApiResponse<bool>.Ok(true, "تم حذف العربة بنجاح");
        }

        public async Task<ApiResponse<CustomerBasket>> GetBasketAsync(string basketId)
        {
            CustomerBasket? basket;
            try
            {
                 basket = await _basketRepo.GetBasketAsync(basketId);
            }
            catch
            {
                return ApiResponse<CustomerBasket>.Fail("مشكلة في التواصل مع Redis");
            }
             
            if (basket is null)
                return ApiResponse<CustomerBasket>.Fail("هذه العربة غير موجودة");
            return ApiResponse<CustomerBasket>.Ok(basket, "تم الحصول علي العربة بنجاح");
        }
    }
}
