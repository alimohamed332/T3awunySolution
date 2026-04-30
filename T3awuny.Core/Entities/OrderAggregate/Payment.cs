//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using T3awuny.Core.Entities.Enums;
//using T3awuny.Core.Entities.UserModule;

//namespace T3awuny.Core.Entities.OrderAggregate
//{
//    public class Payment : BaseEntity
//    {
//        public int Id { get; set; }
//        public int OrderId { get; set; }
//        public Order Order { get; set; }
//        public string PayerId { get; set; }
//        public ApplicationUser Payer { get; set; }
//        public decimal Amount { get; set; }
//        public PaymentMethod Method { get; set; }
//        public PaymentStatus Status { get; set; }
//        public string? TransactionReference { get; set; }
//        public DateTime? PaidAt { get; set; }
//        public DateTime CreatedAt { get; set; }
//    }
//}
