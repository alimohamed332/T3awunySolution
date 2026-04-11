using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace T3awuny.Application.DTOs.Auth
{
    public class ForgotPasswordDto
    {
        [Required]
        public string Email { get; set; } = string.Empty;
    }
}
