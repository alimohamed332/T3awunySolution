using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using T3awuny.Core.Entities;

namespace T3awuny.Core.Specifications
{
    public class TraderSpecifications : BaseSpecifications<TraderProfile>
    {
        public TraderSpecifications() : base() {  /*intialize includes if needed*/}
        public TraderSpecifications(Expression<Func<TraderProfile, bool>> criteria) : base(criteria) { Includes.Add(t => t.User!); }
    }
}
