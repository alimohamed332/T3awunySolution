using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using T3awuny.Core.Entities;

namespace T3awuny.Core.Specifications
{
    public interface ISpecifications<T> where T : BaseEntity
    {
        public Expression<Func<T, bool>>? Criteria { get; } // Expression to filter the data based on the criteria : Where(P => P.Id == 1)
        public List<Expression<Func<T, object>>> Includes { get; } // Expression to include related entities : Include(P => P.Category)

    }
}
