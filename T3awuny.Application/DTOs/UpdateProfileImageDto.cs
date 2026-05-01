using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace T3awuny.Application.DTOs
{
    public class UpdateProfileImageDto
    {
        [Required]
        public IFormFile Image { get; set; } = null!;
    }
}
