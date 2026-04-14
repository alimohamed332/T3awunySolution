using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using T3awuny.Core;
using T3awuny.Core.Entities;
using T3awuny.Core.Repository.Contracts;
using T3awuny.Infrastructure.Data;
using T3awuny.Infrastructure.Repositories;

namespace T3awuny.Infrastructure
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly T3awunyDbContext _dbContext;
        //private Dictionary<string, GenericRepository<BaseEntity>> _repositories;  // Cache for repositories
        //ex: order , GenericRepository<Order> , "Order" , _repositories["Order"]
        private readonly Hashtable _repositories;  // Cache for repositories
        public UnitOfWork(T3awunyDbContext dbContext)
        {
            _dbContext = dbContext;
            //_repositories = new Dictionary<string, GenericRepository<BaseEntity>>();
            _repositories = new Hashtable();

        }

        public IGenericRepository<TEntity> Repository<TEntity>() where TEntity : BaseEntity
        {
            var typeName = typeof(TEntity).Name;

            if(!_repositories.ContainsKey(typeName))
            {
                var repositoryInstance = new GenericRepository<TEntity>(_dbContext);
                //_repositories.Add(typeName, repositoryInstance as GenericRepository<BaseEntity>);
                //_repositories[typeName] = repositoryInstance as GenericRepository<BaseEntity>;

                _repositories[typeName] = repositoryInstance;
                //_repositories.Add(typeName, repositoryInstance);
            }
            return _repositories[typeName] as IGenericRepository<TEntity>;
        }

        public async Task<int> CompleteAsync() => await _dbContext.SaveChangesAsync();

        public async ValueTask DisposeAsync() => await _dbContext.DisposeAsync();

        
    }
}
