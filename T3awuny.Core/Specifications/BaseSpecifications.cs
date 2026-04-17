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
        public Expression<Func<T, bool>>? Criteria { get; private set; }

        public List<Expression<Func<T, object>>> Includes { get; private set; } = new List<Expression<Func<T, object>>>();
        public BaseSpecifications()
        {
            //Criteria = null; // default value for the criteria is null, which means that we want to get all the data without any filtering
        }
        public BaseSpecifications(Expression<Func<T, bool>> criteria)
        {
            Criteria = criteria;
        }
        //a way to handle includes is to add a method that adds the include expression to the list of includes, this way we can chain the method calls and make it more readable
        //public void AddInclude(Expression<Func<T, object>> includeExpression)
        //{
        //   Includes.Add(includeExpression);
        //}
        //another way to handle includes is to intialize the includes in the constructor of the specified class that inherits from the base specifications
    }
}
