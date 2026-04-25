using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using T3awuny.Application.Common;
using T3awuny.Application.DTOs.Product;
using T3awuny.Application.Helpers;
using T3awuny.Core.Entities.Enums;
using T3awuny.Core.Specifications.ProductSpecs;

namespace T3awuny.Application.Contracts
{
    public interface IProductService
    {
        // Farmer actions
        Task<ApiResponse<CreateProductDto>> CreateAsync(string farmerId, CreateProductDto dto);
        Task<ApiResponse<ProductSummaryDto>> UpdateAsync(string farmerId, int productId, UpdateProductDto dto);
        Task<ApiResponse<string>> DeleteAsync(string farmerId, int productId);
        Task<ApiResponse<string>> ChangeStatusAsync(string userId, int productId, ProductStatus status);
        Task<ApiResponse<Pagination<ProductResponseDto>>> GetMyProductsAsync(ProductSpecParams specs);

        // Public actions (buyers browse)
        Task<ApiResponse<ProductResponseDto>> GetByIdAsync(int productId);
        Task<ApiResponse<Pagination<ProductSummaryDto>>> GetAllAsync(ProductSpecParams filter);
        Task<ApiResponse<IReadOnlyList<ProductSummaryDto>>> GetByFarmerAsync(string farmerId);

        // Image management
        Task<ApiResponse<string>> AddImageAsync(string farmerId, int productId, IFormFile image);
        Task<ApiResponse<string>> DeleteImageAsync(string farmerId, int productId, int imageId);
        Task<ApiResponse<string>> SetMainImageAsync(string farmerId, int productId, int imageId);
    }
}
