using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using T3awuny.Core.Entities.OrderAggregate;

namespace T3awuny.Core.Specifications.OrderSpecs
{
    public class OrderSpecifications : BaseSpecifications<Order>
    {
        public OrderSpecifications(Expression<Func<Order,bool>> criteria , bool lighted = false) : base(criteria) 
        {
            if(lighted)
            {
                Includes.Add(o => o.Logistics);
                //Includes.Add(o => o.Payment);
            }
            else
            {
                Includes.Add(o => o.DeliveryMethod);
                Includes.Add(o => o.Logistics);
                Includes.Add(o => o.Items);
                //Includes.Add(o => o.Payment);
            }
           
        }

        public OrderSpecifications(OrderSpecParams specs) 
            :base(O =>
               (!specs.ProductId.HasValue || O.Items.Any(it => it.ItemOrdered.ProductId == specs.ProductId)) &&
               (!specs.MinPrice.HasValue || O.SubTotal >= specs.MinPrice) &&
               (!specs.MaxPrice.HasValue || O.SubTotal <= specs.MaxPrice) &&
               (!specs.OrderStatus.HasValue || O.Status == specs.OrderStatus)&&
               (!specs.PaymentStatus.HasValue || O.PaymentStatus == specs.PaymentStatus)
                 )
        {
            Includes.Add(o => o.DeliveryMethod);
            Includes.Add(o => o.Logistics);
            Includes.Add(o => o.Items);

            if (specs.SortDescending)
                OrderByDesc = o => o.CreatedAt;
            OrderBy = o => o.CreatedAt;

            ApplyPagination((specs.PageIndex - 1) * specs.pageSize, specs.pageSize);
        }
    }
}
