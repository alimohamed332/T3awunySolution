using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace T3awuny.Core.Specifications.UserSpecs
{
    public class AdminUserFilterDto
    {
        private const int MaxPageSize = 20;
        public string? SearchTerm { get; set; }        // name or email
        public string? Role { get; set; }              // Farmer, Trader
        public bool? IsActive { get; set; }
        public bool? IsVerified { get; set; }
        public DateTime? JoinedFrom { get; set; }
        public DateTime? JoinedTo { get; set; }
        public string? SortBy { get; set; }            // joinDate, name, orders
        public bool SortDescending { get; set; } = true;
        public int PageNumber { get; set; } = 1;

        private int pagesize = 10;
        public int PageSize
        {
            get { return pagesize; }
            set { pagesize = value > MaxPageSize ? MaxPageSize : value; }
        }
    }
}
