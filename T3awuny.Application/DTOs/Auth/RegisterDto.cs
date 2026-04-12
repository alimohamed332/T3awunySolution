using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using T3awuny.Core.Entities;

namespace T3awuny.Application.DTOs.Auth
{
    public class RegisterDto
    {
        [Required, StringLength(100)]
        public string FullName { get; set; } = string.Empty;
        [Required, StringLength(50)]
        public string UserName { get; set; } = string.Empty;

        [Required, StringLength(128),EmailAddress]
        public string Email { get; set; } = string.Empty;
        public string? Role { get; set; } = "Farmer";
        //public string? ProfileImageUrl { get; set; }
        public IFormFile? ImageFile { get; set; }
        public virtual ICollection<Address> Addresses { get; set; } = new HashSet<Address>();/////remember to define how to recieve from nasr and how to map to DB
        [Required, StringLength(256)]
        public string Password { get; set; } = string.Empty;
        [Required, StringLength(256),Compare("Password")]
        public string ConfirmedPassword { get; set; } = string.Empty;
    }
}
