using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace T3awuny.Core.Entities.Enums
{
    public enum ProductStatus
    {
        Draft = 0,   // farmer saved but not published
        Active = 1,   // visible to traders on the website
        SoldOut = 2,   // quantity = 0
        Archived = 3,   //  farmer hid it 
        Deleted = 4, //deleted by farmer 
        UnderReview = 5   // admin flagged it
    }
}
