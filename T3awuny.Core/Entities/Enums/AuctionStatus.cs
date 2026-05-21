using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace T3awuny.Core.Entities.Enums
{
    public enum AuctionStatus
    {
        Scheduled = 0,   // created, not started yet
        Active = 1,   // currently accepting bids from buyers
        Ended = 2,   // time expired, winner determined
        Cancelled = 3,   // farmer cancelled before start
        Failed = 4    // ended with no bids or reserve not met
    }
}
