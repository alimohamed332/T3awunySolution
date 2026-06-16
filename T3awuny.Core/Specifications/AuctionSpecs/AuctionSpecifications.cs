using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using T3awuny.Core.Entities.AuctionModule;
using T3awuny.Core.Entities.OrderAggregate;
using T3awuny.Core.Specifications.OrderSpecs;

namespace T3awuny.Core.Specifications.AuctionSpecs
{
    public class AuctionSpecifications : BaseSpecifications<Auction>
    {
        public AuctionSpecifications(Expression<Func<Auction, bool>> criteria, bool lighted = false,bool verylighted = false) : base(criteria)
        {
            if (lighted)
            {
                Includes.Add(a => a.Bids);   
            }
            else if (verylighted)
            {
                Includes.Add(a => a.Product!);
                Includes.Add(a => a.Farmer!);
            }
                
            else
            {
                Includes.Add(a => a.Bids);
                Includes.Add(a => a.Farmer!);
                Includes.Add(a => a.Winner);
                Includes.Add(a => a.Product!);
            }

        }

        public AuctionSpecifications(AuctionSpecParams specs)
            : base(A =>
               (!specs.ProductId.HasValue || A.ProductId == specs.ProductId) &&
               (string.IsNullOrEmpty(specs.FarmerId) || A.FarmerId == specs.FarmerId) &&
               (string.IsNullOrEmpty(specs.WinnerId) || A.WinnerId == specs.WinnerId) &&
               (!specs.Status.HasValue || A.Status == specs.Status)
            )
        {
            Includes.Add(o => o.Bids);
            Includes.Add(o => o.Product!);
            Includes.Add(o => o.Farmer!);
            Includes.Add(o => o.Winner);

            if (!string.IsNullOrEmpty(specs.Sort)) //// CreatedAt , CurrentPrice (the last(highest) price for the auction) , EndDate , StartDate
            {
                switch (specs.Sort)
                {
                    case string sort when sort == "currentPrice" && !specs.SortDescending:
                        OrderBy = p => p.CurrentPrice;
                        break;
                    case string sort when sort == "currentPrice" && specs.SortDescending:
                        OrderByDesc = p => p.CurrentPrice;
                        break;
                    case string sort when sort == "startDate" && !specs.SortDescending:
                        OrderBy = p => p.StartDate!;
                        break;
                    case string sort when sort == "startDate" && specs.SortDescending:
                        OrderByDesc = p => p.StartDate!;
                        break;
                    case string sort when sort == "endDate" && !specs.SortDescending:
                        OrderBy = p => p.EndDate!;
                        break;
                    case string sort when sort == "endDate" && specs.SortDescending:
                        OrderByDesc = p => p.EndDate!;
                        break;
                    default:
                        OrderBy = p => p.CreatedAt;
                        break;
                }
            }
            else
            {
                if(specs.SortDescending)
                    OrderByDesc = a => a.CreatedAt;
                else
                    OrderBy = a => a.CreatedAt;
            }
                


            ApplyPagination((specs.PageIndex - 1) * specs.pageSize, specs.pageSize);
        }
    }
}
