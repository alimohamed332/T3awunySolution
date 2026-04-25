using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using T3awuny.Core.Entities.Enums;

namespace T3awuny.Core.Specifications.ProductSpecs
{
    public class ProductSpecParams
    {
        private const int MaxPageSize = 20;
        public string? Sort { get; set; }  // price, harvest date, name
        public bool SortDescending { get; set; }
        public int? CategoryId { get; set; }
        public string? FarmerId { get; set; }

        private int PageSize = 10;

        public int pageSize
        {
            get { return PageSize; }
            set { PageSize = value > MaxPageSize ? MaxPageSize : value; }
        }
        public int PageIndex { get; set; } = 1;

        public string? Search { get; set; }  //by product name

        public decimal? MinPrice { get; set; }

        public decimal? MaxPrice { get; set; }

        //public string? Governorate { get; set; }     // filter by farmer location
        public ProductStatus? Status { get; set; }

    }
}
