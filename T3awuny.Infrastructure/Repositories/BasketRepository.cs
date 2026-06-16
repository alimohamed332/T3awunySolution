using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using T3awuny.Core.Entities.BasketModule;
using T3awuny.Core.Repository.Contracts;

namespace T3awuny.Infrastructure.Repositories
{
    public class BasketRepository(IConnectionMultiplexer connection) : IBasketRepository
    {
        private readonly IDatabase _database = connection.GetDatabase();
        public async Task<CustomerBasket?> CreateOrUpdateBasketAsync(CustomerBasket basket, TimeSpan? timeToLive = null)
        {
            var IsCreatedOrUpdated = await _database.StringSetAsync(basket.Id, JsonSerializer.Serialize(basket), timeToLive ?? TimeSpan.FromDays(10));
            if (IsCreatedOrUpdated)
                return await GetBasketAsync(basket.Id);
            return null;

        }

        public async Task<bool> DeleteBasketAsync(string id)
        {
            return await _database.KeyDeleteAsync(id);
        }

        public async Task<CustomerBasket?> GetBasketAsync(string id)
        {
            var basket = await _database.StringGetAsync(id);
            if (basket.IsNullOrEmpty)
                return null;
            return JsonSerializer.Deserialize<CustomerBasket>(basket!);
        }
    }
}
