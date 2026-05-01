using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using T3awuny.Application.Common;
using T3awuny.Application.Contracts;
using T3awuny.Application.DTOs.Farmer;
using T3awuny.Application.DTOs.Product;
using T3awuny.Application.Helpers;
using T3awuny.Core.Entities;
using T3awuny.Core.Entities.Enums;
using T3awuny.Core.Specifications.ProductSpecs;

namespace T3awunyWebService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<ApiResponse<Pagination<ProductSummaryDto>>>> GetAllAsync([FromQuery] ProductSpecParams filter)
        {
            var result = await _productService.GetAllAsync(filter);
            if (!result.IsSuccess)
                return BadRequest(result);
            return Ok(result);
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<ProductResponseDto>>> GetByIdAsync(int id)
        {
            var result = await _productService.GetByIdAsync(id);
            if (!result.IsSuccess)
                return BadRequest(result);
            return Ok(result);
        }

        [Authorize]
        [HttpGet("farmers/{id}")]
        public async Task<ActionResult<ApiResponse<IReadOnlyList<ProductSummaryDto>>>> GetByFarmerAsync(string id)
        {
            var result = await _productService.GetByFarmerAsync(id);
            if (!result.IsSuccess)
                return BadRequest(result);
            return Ok(result);
        }

        [Authorize("FarmerOnly")]
        [HttpPost]
        public async Task<ActionResult<ApiResponse<CreateProductDto>>> CreateAsync([FromForm] CreateProductDto dto)
        {
            var farmerId = GetUserIdFromClaims();
            if (string.IsNullOrEmpty(farmerId))
                return BadRequest(ApiResponse<CreateProductDto>.Fail("معرف المستخدم غير موجود في الرمز المميز"));
            var result = await _productService.CreateAsync(farmerId, dto);
            if (!result.IsSuccess)
                return BadRequest(result);
            return Ok(result);
        }

        [Authorize("FarmerOnly")]
        [HttpPut]
        public async Task<ActionResult<ApiResponse<ProductSummaryDto>>> UpdateAsync([FromBody] UpdateProductDto dto)
        {
            var farmerId = GetUserIdFromClaims();
            if (string.IsNullOrEmpty(farmerId))
                return BadRequest(ApiResponse<ProductSummaryDto>.Fail("معرف المستخدم غير موجود في الرمز المميز"));
            var result = await _productService.UpdateAsync(farmerId, dto.Id, dto);
            if (!result.IsSuccess)
                return BadRequest(result);
            return Ok(result);
        }

        [Authorize("FarmerOrAdmin")]
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<string>>> DeleteAsync(int id)
        {
            var farmerId = GetUserIdFromClaims();
            if (string.IsNullOrEmpty(farmerId))
                return BadRequest(ApiResponse<string>.Fail("معرف المستخدم غير موجود في الرمز المميز"));

            var result = await _productService.DeleteAsync(farmerId, id);
            if (!result.IsSuccess)
                return BadRequest(result);
            return Ok(result);                                                         
        }

        [Authorize("FarmerOrAdmin")]
        [HttpPatch("change-status")]
        public async Task<ActionResult<ApiResponse<string>>> ChangeStatusAsync([FromBody] ChangeProductStatusDto dto)
        {
            var userId = GetUserIdFromClaims();
            if (string.IsNullOrEmpty(userId))
                return BadRequest(ApiResponse<string>.Fail("معرف المستخدم غير موجود في الرمز المميز"));
            var result = await _productService.ChangeStatusAsync(userId, dto.ProductId, dto.ProductStatus);

            if(!result.IsSuccess)
                return BadRequest(result);
            return Ok(result);
        }

        [Authorize("FarmerOnly")]
        [HttpGet("my-products")]
        public async Task<ActionResult<ApiResponse<Pagination<ProductResponseDto>>>> GetMyProductsAsync([FromQuery] ProductSpecParams specs)
        {
            var farmerId = GetUserIdFromClaims();
            if (string.IsNullOrEmpty(farmerId))
                return BadRequest(ApiResponse<Pagination<ProductResponseDto>>.Fail("معرف المستخدم غير موجود في الرمز المميز"));
            specs.FarmerId = farmerId;
            var result = await _productService.GetMyProductsAsync(specs);
            if(!result.IsSuccess)
                return BadRequest(result);
            return Ok(result);
        }

        [Authorize("FarmerOnly")]
        [HttpPost("{productId}/images")]
        public async Task<ActionResult<ApiResponse<string>>> AddImageAsync(int productId, [FromForm] AddProductImageDto dto)
        {
            var farmerId = GetUserIdFromClaims();
            if (string.IsNullOrEmpty(farmerId))
                return BadRequest(ApiResponse<string>.Fail("معرف المستخدم غير موجود في الرمز المميز"));

            if (dto.Image is null || dto.Image.Length == 0)
                return BadRequest(ApiResponse<string>.Fail("يرجى تحميل صورة صحيحة"));

            var result = await _productService.AddImageAsync(farmerId,productId,dto.Image);
            if(!result.IsSuccess)
                return BadRequest(result);
            return Ok(result);
        }
        [Authorize("FarmerOnly")]
        [HttpPatch("{productId}/images/{imageId}/set-main")]
        public async Task<ActionResult<ApiResponse<string>>> SetMainImageAsync(int productId, int imageId)
        {
            var farmerId = GetUserIdFromClaims();
            if (string.IsNullOrEmpty(farmerId))
                return BadRequest(ApiResponse<string>.Fail("معرف المستخدم غير موجود في الرمز المميز"));

            var result = await _productService.SetMainImageAsync(farmerId,productId,imageId);
            if(!result.IsSuccess)
                return BadRequest(result);
            return Ok(result);
        }

        [Authorize("FarmerOnly")]
        [HttpDelete("{productId}/images/{imageId}")]
        public async Task<ActionResult<ApiResponse<string>>> DeleteImageAsync(int productId, int imageId)
        {
            var farmerId = GetUserIdFromClaims();
            if (string.IsNullOrEmpty(farmerId))
                return BadRequest(ApiResponse<string>.Fail("معرف المستخدم غير موجود في الرمز المميز"));

            var result = await _productService.DeleteImageAsync(farmerId,productId,imageId);
            if(!result.IsSuccess)
                return BadRequest(result);
            return Ok(result);
        }
        private string GetUserIdFromClaims()
        {
            return User.Claims.FirstOrDefault(c => c.Type == "uid")?.Value ?? string.Empty;
        }
    }
}
