using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace T3awuny.Core.Entities.Enums
{
    public enum LogisticsStatus
    {
        NotScheduled = 0,
        Scheduled = 1,
        PickedUp = 2,
        InTransit = 3,
        Delivered = 4,
        Failed = 5
    }
}
