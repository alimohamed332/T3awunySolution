using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using T3awuny.Core.Entities;

namespace T3awuny.Core.Specifications
{
    public class FarmerSpecifications : BaseSpecifications<FarmerProfile>
    {
        public FarmerSpecifications() : base() {  /*intialize includes if needed*/}
        public FarmerSpecifications(Expression<Func<FarmerProfile, bool>> criteria) : base(criteria) { Includes.Add(f => f.User!); }
    }
}
