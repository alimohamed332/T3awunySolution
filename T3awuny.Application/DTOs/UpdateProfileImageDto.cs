using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace T3awuny.Application.DTOs
{
    public class UpdateProfileImageDto
    {
        public string UserId { get; set; } = string.Empty;
        public IFormFile Image { get; set; } = null!;
    }
}
