using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using T3awuny.Application.Common;
using T3awuny.Application.Contracts;
using T3awuny.Core;
using T3awuny.Core.Entities;

namespace T3awuny.Application.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoryService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<IReadOnlyList<Category>>> GetCategoriesAsync()
        {
           var categories = await _unitOfWork.Repository<Category>().GetAllAsync();
            if (!categories.Any())
                return ApiResponse<IReadOnlyList<Category>>.Fail("لا يوجد فئات متاحة لعرضها");

            return ApiResponse<IReadOnlyList<Category>>.Ok(categories, "تم العثور علي الفئات المتاحة بنجاح");
        }

        public async Task<ApiResponse<Category>> GetCategoryByIdAsync(int id)
        {
            var category = await _unitOfWork.Repository<Category>().GetByIdAsync(id);
            if (category is null)
                return ApiResponse<Category>.Fail("هذه الفئة غير موجودة");

            return ApiResponse<Category>.Ok(category, "تم العثور علي الفئة بنجاح");
        }
    }
}
