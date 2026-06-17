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

        public async Task<ApiResponse<CategoryDto>> GetCategoryByIdAsync(int id)
        {
            var category = await _unitOfWork.Repository<Category>().GetByIdAsync(id);
            if (category is null)
                return ApiResponse<CategoryDto>.Fail("هذه الفئة غير موجودة");

            return ApiResponse<CategoryDto>.Ok(_mapper.Map<CategoryDto>(category), "تم العثور علي الفئة بنجاح");
        }
        public async Task<ApiResponse<CategoryDto>> CreateCategory(CreateCategoryDto dto)
        {
            var category = _mapper.Map<Category>(dto);
            await _unitOfWork.Repository<Category>().AddAsync(category);

            if ( await _unitOfWork.CompleteAsync() <= 0)
                return ApiResponse<CategoryDto>.Fail("فشل حفظ الفئة الجديدة حاول لاحقاً");

            return ApiResponse<CategoryDto>.Ok(_mapper.Map<CategoryDto>(category),"تم أضافة الفئة الجديدة بنجاح");
        }

        public async Task<ApiResponse<bool>> DeleteCategory(int categoryId)
        {
            var category = await _unitOfWork.Repository<Category>().GetByIdAsync(categoryId);
            if (category is null)
                return ApiResponse<bool>.Fail("لا يوجد فئة بهذا المعرف");

            _unitOfWork.Repository<Category>().Delete(category);

            if(await _unitOfWork.CompleteAsync() <= 0) // مش هيمسحها لو ليها منتجات بتشير ليها
                return ApiResponse<bool>.Fail("فشل مسح الفئة");

            return ApiResponse<bool>.Ok(true, "تم مسح الفئة بنجاح");
        }

        public async Task<ApiResponse<CategoryDto>> UpdateCategory(int categoryId,UpdateCategoryDto dto)
        {
            var category = await _unitOfWork.Repository<Category>().GetByIdAsync(categoryId);
            if (category is null)
                return ApiResponse<CategoryDto>.Fail("لا يوجد فئة بهذا المعرف");
            category.Name = string.IsNullOrEmpty(dto.Name) ? category.Name : dto.Name;
            category.NameAr = string.IsNullOrEmpty(dto.NameAr) ? category.NameAr : dto.NameAr;
            category.IconUrl = string.IsNullOrEmpty(dto.IconUrl) ? category.IconUrl : dto.IconUrl;
            category.ParentCategoryId = dto.ParentCategoryId.HasValue ? dto.ParentCategoryId.Value : category.ParentCategoryId;
            _unitOfWork.Repository<Category>().Update(category);

            if (await _unitOfWork.CompleteAsync() <= 0)
                return ApiResponse<CategoryDto>.Fail("فشل تحديث الفئة حاول لاحقاً");
            return ApiResponse<CategoryDto>.Ok(_mapper.Map<CategoryDto>(category),"تم تحديث الفئة بنجاح");

        }
    }
}
