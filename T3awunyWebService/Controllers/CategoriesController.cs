using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using T3awuny.Application.Common;
using T3awuny.Application.Contracts;
using T3awuny.Application.DTOs.Category;
using T3awuny.Core.Entities;

namespace T3awunyWebService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }
        [HttpGet]
        public async Task<ActionResult<ApiResponse<IReadOnlyList<CategoryDto>>>> GetCategories()
        {
            var result = await _categoryService.GetCategoriesAsync();
            if (!result.IsSuccess)
                return NotFound(result);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<CategoryDto>>> GetCategory(int id)
        {
            var result = await _categoryService.GetCategoryByIdAsync(id);
            if (!result.IsSuccess)
                return NotFound(result);
            return Ok(result);
        }
        [Authorize("AdminOnly")]
        [HttpPost]
        public async Task<ActionResult<ApiResponse<CategoryDto>>> CreateCategory([FromBody] CreateCategoryDto dto)
        {
            var result = await _categoryService.CreateCategory(dto);
            if (!result.IsSuccess)
                return BadRequest(result);
            return Ok(result);
        }
        [Authorize("AdminOnly")]
        [HttpPut("{categoryId}")]
        public async Task<ActionResult<ApiResponse<CategoryDto>>> UpdateCategory(int categoryId, [FromBody] UpdateCategoryDto dto)
        {
            var result = await _categoryService.UpdateCategory(categoryId, dto);
            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        [Authorize("AdminOnly")]
        [HttpDelete("{categoryId}")]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteCategory(int categoryId)
        {
            var result = await _categoryService.DeleteCategory(categoryId);
            if (!result.IsSuccess)
                return BadRequest(result);
            return Ok(result);
        }
    }
}
