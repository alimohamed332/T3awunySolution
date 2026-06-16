using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using T3awuny.Core.Entities;
using T3awuny.Core.Repository.Contracts;
using T3awuny.Core.Specifications;
using T3awuny.Infrastructure.Data;
using T3awuny.Infrastructure.Helpers;

namespace T3awuny.Infrastructure.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
    {
        protected readonly T3awunyDbContext _dbContext;
        protected readonly DbSet<T> _dbSet;

        public GenericRepository(T3awunyDbContext dbContext)
        {
            _dbContext = dbContext;
            _dbSet = _dbContext.Set<T>();
        }

        public async Task<T?> GetByIdAsync(int id)
        {
            //if (typeof(T) == typeof(ApplicationUser))
                //return await _dbContext.Set<ApplicationUser>().Where(a => a.Id == id).Include(a => a.Addresses).FirstOrDefaultAsync(a => a.Id == id) as T;
            return await _dbSet.FindAsync(id);
        }
        public async Task<T?> GetByIdAsync(string id)
        {
            //if (typeof(T) == typeof(ApplicationUser))
            //return await _dbContext.Set<ApplicationUser>().Where(a => a.Id == id).Include(a => a.Addresses).FirstOrDefaultAsync(a => a.Id == id) as T;
            return await _dbSet.FindAsync(id);
        }
        public async Task<T?> GetByIdWithSpecAsync(ISpecifications<T> spec)
        {
             return await ApplySpecification(spec).FirstOrDefaultAsync();  
        }

        public async Task<IReadOnlyList<T>> GetAllAsync()
        {
            //if (typeof(T) == typeof(ApplicationUser))
               //return await _dbContext.Set<ApplicationUser>().Where(a => a.Id == id).Include(a => a.Addresses).AsNoTracking().ToListAsync() as IReadOnlyList<T>;
            return await _dbSet.AsNoTracking().ToListAsync();
        }

        public async Task<IReadOnlyList<T>> GetAllWithSpecAsync(ISpecifications<T> spec)
        {
            return await ApplySpecification(spec).AsNoTracking().ToListAsync();
        }

        public async Task<T> AddAsync(T entity)
        {
            var result = await _dbSet.AddAsync(entity);
            return result.Entity;
        }

        public T Update(T entity)
        {
            var result = _dbSet.Update(entity);
            return result.Entity;
        }
        public void Delete(T entity)
        {
            _dbSet.Remove(entity);
        }

        public async Task<int> GetCountAsync(ISpecifications<T> spec)
        {
            return await ApplySpecification(spec).CountAsync();
        }

        private IQueryable<T> ApplySpecification(ISpecifications<T> spec)
        {
            return SpecificationsEvaluator<T>.GetQuery(_dbSet, spec);
        }
    }
}
