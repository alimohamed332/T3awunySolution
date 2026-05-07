using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using T3awuny.Application.Common;
using T3awuny.Application.Contracts;
using T3awuny.Application.DTOs.Category;
using T3awuny.Core;
using T3awuny.Core.Entities;

namespace T3awuny.Application.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CategoryService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ApiResponse<IReadOnlyList<CategoryDto>>> GetCategoriesAsync()
        {
           var categories = await _unitOfWork.Repository<Category>().GetAllAsync();
            if (!categories.Any())
                return ApiResponse<IReadOnlyList<CategoryDto>>.Fail("لا يوجد فئات متاحة لعرضها");
            var categoriesDto = categories.Select(c => _mapper.Map<CategoryDto>(c)).ToList();

            return ApiResponse<IReadOnlyList<CategoryDto>>.Ok(categoriesDto, "تم العثور علي الفئات المتاحة بنجاح");
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
