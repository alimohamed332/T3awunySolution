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
        /// <summary>
        /// Possible values : price, date (sort by harvest date), name (default)
        /// </summary>
        public string? Sort { get; set; }  // price, harvest date, name
        /// <summary>
        /// true : sort des , false (default) sort asc
        /// </summary>
        public bool SortDescending { get; set; }
        public int? CategoryId { get; set; }
        public string? FarmerId { get; set; }
        /// <summary>
        /// true : get products that farmer put auctions on it , false get products has no auctions , null get all products
        /// </summary>
        public bool? HasActiveAuction { get; set; } = null;

        private int PageSize = 10;
        /// <summary>
        /// Pagination Default page size 10 , and max 20 and 
        /// </summary>
        public int pageSize
        {
            get { return PageSize; }
            set { PageSize = value > MaxPageSize ? MaxPageSize : value; }
        }
        /// <summary>
        /// default page index 1
        /// </summary>
        public int PageIndex { get; set; } = 1;
        /// <summary>
        /// Search by product name
        /// </summary>
        public string? Search { get; set; }  //by product name

        public decimal? MinPrice { get; set; }

        public decimal? MaxPrice { get; set; }

        //public string? Governorate { get; set; }     // filter by farmer location
        /// <summary>
        /// Draft = 0 farmer saved but not published 
        /// Active = 1 visible to traders on the website 
        /// SoldOut = 2 quantity = 0
        /// Archived = 3 farmer hid it
        /// Deleted = 4 deleted by farmer 
        /// UnderReview = 5 admin flagged it
        /// </summary>
        public ProductStatus? Status { get; set; }

    }
}
