using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace T3awuny.Application.DTOs.Product
{
    public class UpdateProductImageDto
    {
        [Required]
        public int ProductId { get; set; }
        [Required]
        public int ImageId { get; set; } 
    }
}
