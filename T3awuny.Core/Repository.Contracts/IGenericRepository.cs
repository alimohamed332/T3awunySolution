using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using T3awuny.Core.Entities;
using T3awuny.Core.Specifications;

namespace T3awuny.Core.Repository.Contracts
{
    public interface IGenericRepository<T> where T : BaseEntity
    {
        Task<T?> GetByIdAsync(int id);
        Task<T?> GetByIdAsync(string id);
        //Task<T> GetByNameAsync(string name);
        Task<IReadOnlyList<T>> GetAllAsync();
        Task<T?> GetByIdWithSpecAsync(ISpecifications<T> spec);
        Task<IReadOnlyList<T>> GetAllWithSpecAsync(ISpecifications<T> spec);
        Task<int> GetCountAsync(ISpecifications<T> spec);
        Task<T> AddAsync(T entity);
        T Update(T entity);
        void Delete(T entity);
    }
}
