using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using T3awuny.Core.Entities;

namespace T3awuny.Core.Repository.Contracts
{
    public interface IGenericRepository<T> where T : BaseEntity
    {
        Task<T?> GetByIdAsync(int id);
        //Task<T> GetByNameAsync(string name);
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> AddAsync(T entity);
        T Update(T entity);
        void Delete(T entity);
        Task<bool> SaveAsync();
    }
}
