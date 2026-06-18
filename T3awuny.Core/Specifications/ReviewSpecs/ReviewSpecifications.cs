using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using T3awuny.Core.Entities.ReviewModule;

namespace T3awuny.Core.Specifications.ReviewSpecs
{
    public class ReviewSpecifications :  BaseSpecifications<Review>
    {
        public ReviewSpecifications(Expression<Func<Review, bool>> criteria) : base(criteria) 
        {
            Includes.Add(r => r.Reviewer);
            Includes.Add(r => r.TargetUser);
        }
    }
}
