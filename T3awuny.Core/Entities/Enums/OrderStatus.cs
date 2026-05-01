using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace T3awuny.Core.Entities.Enums
{
    public enum OrderStatus
    {
        Pending = 0,  // buyer placed, waiting farmer response
        Confirmed = 1,  // farmer accepted
        Preparing = 2,  // farmer is preparing the order
        ReadyForPickup = 3, // ready for logistics
        InDelivery = 4,  // driver picked up
        Delivered = 5,  // buyer received
        Cancelled = 6,  // cancelled by buyer or farmer
        Rejected = 7   // farmer rejected
    }
}
