using T3awuny.Application.Common;
using T3awuny.Application.DTOs.Category;

namespace T3awuny.Application.Contracts
{
    public interface ICategoryService
    {
        Task<ApiResponse<IReadOnlyList<CategoryDto>>> GetCategoriesAsync();
        Task<ApiResponse<CategoryDto>> GetCategoryByIdAsync(int id);
        Task<ApiResponse<CategoryDto>> CreateCategory(CreateCategoryDto dto );
        Task<ApiResponse<CategoryDto>> UpdateCategory(int categoryId,UpdateCategoryDto dto );
        Task<ApiResponse<bool>> DeleteCategory(int categoryId);
    }
}
