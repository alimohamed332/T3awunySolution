using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using T3awuny.Core.Entities;
using T3awuny.Core.Specifications;

namespace T3awuny.Infrastructure.Helpers
{
    internal static class SpecificationsEvaluator<TEntity> where TEntity : BaseEntity
    {
        public static IQueryable<TEntity> GetQuery(IQueryable<TEntity> inputQuery, ISpecifications<TEntity> spec)
        {
            var query = inputQuery; //_dbContext.Set<Product>();

            if (spec.Criteria != null)
            {
                query = query.Where(spec.Criteria); //_dbContext.Set<Product>().Where(p => p.BrandId == 1);
            }

            if (spec.OrderBy is not null)
                query = query.OrderBy(spec.OrderBy);
            else if (spec.OrderByDesc is not null)
                query = query.OrderByDescending(spec.OrderByDesc);

            if (spec.IsPaginationEnabled)
                query = query.Skip(spec.Skip).Take(spec.Take);

            query = spec.Includes.Aggregate(query, (current, includeExpression) => current.Include(includeExpression)); //_dbContext.Set<Product>().Where(p => p.BrandId == 1);.Include(p => p.Brand).Include(p => p.ProductType);


            return query;
        }
    }
}
