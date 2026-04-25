using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace T3awuny.Application.DTOs.Product
{
    public class AddProductImageDto
    {
        [Required]
        public int ProductId { get; set; }
        [Required]
        public required IFormFile Image {  get; set; }
    }
}
