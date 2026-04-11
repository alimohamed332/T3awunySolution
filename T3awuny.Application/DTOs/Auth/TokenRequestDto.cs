using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace T3awuny.Application.DTOs.Auth
{
    public class TokenRequestDto
    {
        [Required,EmailAddress, StringLength(128)]
        public string Email { get; set; } = string.Empty;
        [Required,PasswordPropertyText, StringLength(256)]
        public string Password { get; set; } = string.Empty;
    }
}
