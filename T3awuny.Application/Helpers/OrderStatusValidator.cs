using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using T3awuny.Core.Entities.Enums;

namespace T3awuny.Application.Helpers
{
    public static class OrderStatusValidator
    {
        private static readonly Dictionary<OrderStatus, List<OrderStatus>> _allowedTransitions
            = new()
            {
            { OrderStatus.Pending,        new() { OrderStatus.Confirmed, OrderStatus.Rejected, OrderStatus.Cancelled } },
            { OrderStatus.Confirmed,      new() { OrderStatus.Preparing, OrderStatus.Cancelled } },
            { OrderStatus.Preparing,      new() { OrderStatus.ReadyForPickup, OrderStatus.Cancelled } },
            { OrderStatus.ReadyForPickup, new() { OrderStatus.InDelivery } },
            { OrderStatus.InDelivery,     new() { OrderStatus.Delivered, /*OrderStatus.Failed*/ } },
            { OrderStatus.Delivered,      new() { } },
            { OrderStatus.Cancelled,      new() { } },
            { OrderStatus.Rejected,       new() { } }
            };

        public static bool IsValidTransition(OrderStatus current, OrderStatus next)
            => _allowedTransitions.TryGetValue(current, out var allowed)
               && allowed.Contains(next);
    }
}
