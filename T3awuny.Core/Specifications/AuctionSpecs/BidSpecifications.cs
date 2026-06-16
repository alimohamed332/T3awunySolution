using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using T3awuny.Core.Entities.AuctionModule;

namespace T3awuny.Core.Specifications.AuctionSpecs
{
    public class BidSpecifications : BaseSpecifications<Bid>
    {
        public BidSpecifications(Expression<Func<Bid,bool>> criteria) : base(criteria)
        {
            Includes.Add(b => b.Bidder);
        }
    }
}
