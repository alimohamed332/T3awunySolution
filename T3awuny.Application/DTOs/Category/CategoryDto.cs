using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace T3awuny.Application.DTOs.Category
{
    public class CategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? NameAr { get; set; }
        public string? IconUrl { get; set; }
        public int? ParentCategoryId { get; set; } 
    }
}
