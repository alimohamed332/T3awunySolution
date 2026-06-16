using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using T3awuny.Core.Entities.OrderAggregate;

namespace T3awuny.Core.Specifications
{
    public class PaymentSpecifications : BaseSpecifications<Payment>
    {
        public PaymentSpecifications(Expression<Func<Payment,bool>> criteria) : base(criteria)
        {
            Includes.Add(p => p.Payer);
        }
    }
}
