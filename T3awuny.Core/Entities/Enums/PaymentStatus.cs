using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace T3awuny.Core.Entities.Enums
{
    public enum PaymentStatus
    {
        Unpaid = 0,
        Paid = 1,
        Refunded = 2,
        Failed = 3   // حصل مشكلة قبل الدفع او تم رفض الاوردر او الغائه
    }
}
