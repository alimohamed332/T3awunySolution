using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace T3awuny.Application.DTOs.Admin
{
    public class DashboardStatsDto
    {
        // Users
        public int TotalUsers { get; set; }
        public int TotalFarmers { get; set; }
        public int TotalTraders { get; set; }
        //public int TotalConsumers { get; set; }
        public int PendingVerifications { get; set; }
        public int BannedUsers { get; set; }

        // Products
        public int TotalProducts { get; set; }
        public int ActiveProducts { get; set; }
        public int SoldOutProducts { get; set; }
        public int UnderReviewProducts { get; set; }

        // Orders
        public int TotalOrders { get; set; }
        public int PendingOrders { get; set; }
        public int DeliveredOrders { get; set; }
        public int CancelledOrders { get; set; }
        //public decimal TotalRevenue { get; set; }
        //public decimal RevenueThisMonth { get; set; }

        // Auctions
        public int TotalAuctions { get; set; }
        public int ActiveAuctions { get; set; }
        public int EndedAuctions { get; set; }

        // Community
        public int TotalPosts { get; set; }
        public int PendingReviews { get; set; }

        // Growth (last 30 days)
        public int NewUsersThisMonth { get; set; }
        public int NewOrdersThisMonth { get; set; }
        public int NewProductsThisMonth { get; set; }
    }
}
