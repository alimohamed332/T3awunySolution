using T3awuny.Core.Entities.Enums;
using T3awuny.Core.Entities.UserModule;

namespace T3awuny.Core.Entities.OrderAggregate
{
    public class Logistics : BaseEntity
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public virtual Order Order { get; set; } = default!;
        public int PickupAddressId { get; set; }
        public virtual Address PickupAddress { get; set; } = default!;
        public int DeliveryAddressId { get; set; }
        public virtual Address DeliveryAddress { get; set; } = default!;
        public string? DriverName { get; set; }//
        public string? DriverPhone { get; set; }//
        public LogisticsStatus Status { get; set; }
        public DateTime? EstimatedDelivery { get; set; }
        public DateTime? ActualDelivery { get; set; } //
        public string? Notes { get; set; } //
    }
}
