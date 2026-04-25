using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using T3awuny.Core.Entities;

namespace T3awuny.Core.Specifications
{
    public class BaseSpecifications<T> : ISpecifications<T> where T : BaseEntity
    {
        public Expression<Func<T, bool>>? Criteria { get; protected set; }

        public List<Expression<Func<T, object>>> Includes { get; protected set; } = new List<Expression<Func<T, object>>>();

        public Expression<Func<T, object>> OrderBy { get; protected set; }

        public Expression<Func<T, object>> OrderByDesc { get; protected set; }

        public int Take { get; private set; }

        public int Skip { get; private set; }

        public bool IsPaginationEnabled { get; private set; }

        public BaseSpecifications()
        {
            //Criteria = null; // default value for the criteria is null, which means that we want to get all the data without any filtering
        }
        public BaseSpecifications(Expression<Func<T, bool>> criteria)
        {
            Criteria = criteria;
        }
        public void ApplyPagination(int skip, int take)
        {
            IsPaginationEnabled = true;
            Skip = skip;
            Take = take;

        }
        //a way to handle includes is to add a method that adds the include expression to the list of includes, this way we can chain the method calls and make it more readable
        //public void AddInclude(Expression<Func<T, object>> includeExpression)
        //{
        //   Includes.Add(includeExpression);
        //}
        //another way to handle includes is to intialize the includes in the constructor of the specified class that inherits from the base specifications
    }
}
