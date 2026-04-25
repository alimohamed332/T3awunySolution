using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using T3awuny.Application.Common;
using T3awuny.Core.Entities;

namespace T3awuny.Application.Contracts
{
    public interface ICategoryService
    {
        Task<ApiResponse<IReadOnlyList<Category>>> GetCategoriesAsync();
        Task<ApiResponse<Category>> GetCategoryByIdAsync(int id);
    }
}
