using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using T3awuny.Core.Entities.Enums;

namespace T3awuny.Application.DTOs.Product
{
    public class ChangeProductStatusDto
    {
        [Required]
        public int ProductId { get; set; }
        [Required]
        public ProductStatus ProductStatus { get; set; }
    }
}
