using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using T3awuny.Application.Common;
using T3awuny.Application.Contracts;
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

        public async Task<ApiResponse<CustomerBasket>> CreateOrUpdateBasketAsync(CustomerBasket basket)
        {
            var basketFromRepo = await _basketRepo.CreateOrUpdateBasketAsync(basket);
            if (basketFromRepo is null)
                return ApiResponse<CustomerBasket>.Fail("هذه العربة غير موجودة");
            return ApiResponse<CustomerBasket>.Ok(basketFromRepo,"تم اضافة العربة بنجاح");
        }

        public async Task<ApiResponse<bool>> DeleteBasketAsync(string basketId)
        {
            var basketDeleted = await _basketRepo.DeleteBasketAsync(basketId);
            if(!basketDeleted)
                return ApiResponse<bool>.Fail("هذه العربة غير موجودة او فشل حذفها حاول مرة اخري لاحقاً");
            return ApiResponse<bool>.Ok(true, "تم حذف العربة بنجاح");
        }

        public async Task<ApiResponse<CustomerBasket>> GetBasketAsync(string basketId)
        {
            var basket = await _basketRepo.GetBasketAsync(basketId);
            if (basket is null)
                return ApiResponse<CustomerBasket>.Fail("هذه العربة غير موجودة");
            return ApiResponse<CustomerBasket>.Ok(basket, "تم الحصول علي العربة بنجاح");
        }
    }
}
