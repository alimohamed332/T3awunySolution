using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using T3awuny.Core.Entities;

namespace T3awuny.Core.Specifications
{
    public class AddressSpecifications : BaseSpecifications<Address>
    {
        public AddressSpecifications():base() {  /*intialize includes if needed*/}
        public AddressSpecifications(Expression<Func<Address, bool>> criteria):base(criteria) { /*intialize includes if needed*/}
    }
}
