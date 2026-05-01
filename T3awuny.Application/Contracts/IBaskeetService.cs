using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using T3awuny.Application.Common;
using T3awuny.Core.Entities.BasketModule;

namespace T3awuny.Application.Contracts
{
    public interface IBaskeetService
    {
        Task<ApiResponse<CustomerBasket>> GetBasketAsync(string basketId);  
        Task<ApiResponse<CustomerBasket>> CreateOrUpdateBasketAsync(CustomerBasket basket);
        Task<ApiResponse<bool>> DeleteBasketAsync(string basketId);
    }
}
