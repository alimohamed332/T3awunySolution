using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using T3awuny.Core.Entities.Enums;

namespace T3awuny.Core.Specifications.OrderSpecs
{
    public class OrderSpecParams
    {
        private const int MaxPageSize = 20;
        //public string? Sort { get; set; }  
        public bool SortDescending { get; set; }
        //public int? CategoryId { get; set; }
        //I will implement default sort with Order Creation Date
        private int PageSize = 10;

        public int pageSize
        {
            get { return PageSize; }
            set { PageSize = value > MaxPageSize ? MaxPageSize : value; }
        }
        public int PageIndex { get; set; } = 1;

        public int? ProductId { get; set; }  

        public decimal? MinPrice { get; set; }

        public decimal? MaxPrice { get; set; }

        public OrderStatus? OrderStatus { get; set; }

        public PaymentStatus? PaymentStatus { get; set; }

    }
}
