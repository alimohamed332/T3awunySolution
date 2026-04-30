using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace T3awuny.Core.Entities.BasketModule
{
    public class BasketItem
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = default!;
        public string PictureUrl { get; set; } = default!;
        [Range(1, double.MaxValue,ErrorMessage ="السعر لا يمكن ان يكون اقل من 1")]
        public decimal Price { get; set; }
        [Range(1, double.MaxValue, ErrorMessage = "الكمية لا يمكن ان تكون اقل من 1")]
        public int Quantity { get; set; }
    }
}
