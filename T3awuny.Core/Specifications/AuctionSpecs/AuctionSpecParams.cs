using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using T3awuny.Core.Entities.Enums;

namespace T3awuny.Core.Specifications.AuctionSpecs
{
    public class AuctionSpecParams
    {
        //Ceriteria
        public int? ProductId { get; set; }
        public string? FarmerId { get; set; }
        public string? WinnerId { get; set; }
        public AuctionStatus? Status { get; set; }
        //Ordering    default with create at
        public string? Sort { get; set; }  // CreatedAt , CurrentPrice (the last(highest) price for the auction) , EndDate , StartDate
        public bool SortDescending { get; set; }

        //Pagination
        private const int MaxPageSize = 20;
        private int PageSize = 10;
        public int pageSize
        {
            get { return PageSize; }
            set { PageSize = value > MaxPageSize ? MaxPageSize : value; }
        }
        public int PageIndex { get; set; } = 1;
    }
}
