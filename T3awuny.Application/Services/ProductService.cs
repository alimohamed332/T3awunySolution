using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using T3awuny.Application.Common;
using T3awuny.Application.Contracts;
using T3awuny.Application.DTOs.Product;
using T3awuny.Application.Helpers;
using T3awuny.Core;
using T3awuny.Core.Entities;
using T3awuny.Core.Entities.Enums;
using T3awuny.Core.Specifications;
using T3awuny.Core.Specifications.ProductSpecs;
using static System.Net.Mime.MediaTypeNames;

namespace T3awuny.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IFileStorageService _imageService;

        public ProductService(UserManager<ApplicationUser> userManager, IUnitOfWork unitOfWork, IMapper mapper, IFileStorageService imageService)
        {
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _imageService = imageService;
        }

        public async Task<ApiResponse<Pagination<ProductSummaryDto>>> GetAllAsync(ProductSpecParams filter)
        {
            var productSpec = new ProductSpecifications(filter,true);
            var products = await _unitOfWork.Repository<Product>().GetAllWithSpecAsync(productSpec);
            var countSpec = new BaseSpecifications<Product>(productSpec.Criteria!);
            var count = await _unitOfWork.Repository<Product>().GetCountAsync(countSpec);
            if (!products.Any())
                return ApiResponse<Pagination<ProductSummaryDto>>.Fail("لا يوجد منتجات لعرضها");
            var productsDto = products.Select(p => _mapper.Map<ProductSummaryDto>(p)).ToList();
            var pagination = new Pagination<ProductSummaryDto>(filter.PageIndex,filter.pageSize,count,productsDto);

            return ApiResponse<Pagination<ProductSummaryDto>>.Ok(pagination, "تم العثور علي المنتجات بنجاح");
        }

        public async Task<ApiResponse<IEnumerable<ProductSummaryDto>>> GetByFarmerAsync(string farmerId)
        {
            var farmer = await _userManager.FindByIdAsync(farmerId);
            if (farmer is null)
                return ApiResponse<IEnumerable<ProductSummaryDto>>.Fail("هذا المزارع غير موجود");
            var productSpec = new ProductSpecifications(p => p.FarmerId == farmerId);
            var products = await _unitOfWork.Repository<Product>().GetAllWithSpecAsync(productSpec);
            if(!products.Any())
                return ApiResponse<IEnumerable<ProductSummaryDto>>.Fail("لا يوجد منتجات معروضة لهذا المزارع");

            var productDtos = products.Select(p =>_mapper.Map<ProductSummaryDto>(p));
            return ApiResponse<IEnumerable<ProductSummaryDto>>.Ok(productDtos, "تم العثور علي محاصيل المزارع بنجاح");
        }

        public async Task<ApiResponse<ProductResponseDto>> GetByIdAsync(int productId)
        {
            var productSpec = new ProductSpecifications(p => p.Id == productId, false);
            var product = await _unitOfWork.Repository<Product>().GetByIdWithSpecAsync(productSpec);
            if (product is null)
                return ApiResponse<ProductResponseDto>.Fail("هذا المنتج غير موجود");
            var productDto = _mapper.Map<ProductResponseDto>(product);
            return ApiResponse<ProductResponseDto>.Ok(productDto, "تم العثور علي المنتج بنجاح");
        }

        public async Task<ApiResponse<Pagination<ProductResponseDto>>> GetMyProductsAsync(ProductSpecParams specs)
        {
            var farmer = await _userManager.FindByIdAsync(specs.FarmerId ?? "");
            if (farmer is null)
                return ApiResponse<Pagination<ProductResponseDto>>.Fail("هذا المزارع غير موجود");
            var roles = await _userManager.GetRolesAsync(farmer);
            if (!roles.Any() || !roles.Contains("Farmer"))
                return ApiResponse<Pagination<ProductResponseDto>>.Fail("هذا المستخدم ليس مزارع");

            var productSpec = new ProductSpecifications(specs,true);
            var products = await _unitOfWork.Repository<Product>().GetAllWithSpecAsync(productSpec);

            var countSpec = new BaseSpecifications<Product>(productSpec.Criteria!);
            var count = await _unitOfWork.Repository<Product>().GetCountAsync(countSpec);
            if (!products.Any())
                return ApiResponse<Pagination<ProductResponseDto>>.Fail("لا يوجد منتجات معروضة لهذا المزارع");
            var productDtos = products.Select(p => _mapper.Map<ProductResponseDto>(p)).ToList();

            var pagination = new Pagination<ProductResponseDto>(specs.PageIndex, specs.pageSize, count, productDtos);

            return ApiResponse<Pagination<ProductResponseDto>>.Ok(pagination, "تم العثور علي محاصيل المزارع بنجاح");
        }

        public async Task<ApiResponse<string>> ChangeStatusAsync(string userId, int productId, ProductStatus status)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return ApiResponse<string>.Fail("هذا المستخدم غير موجود");
            var product = await _unitOfWork.Repository<Product>().GetByIdAsync(productId);
            if (product == null)
                return ApiResponse<string>.Fail("هذا المنتج غير موجود");

            var roles = await _userManager.GetRolesAsync(user);
            if (!roles.Any())
                return ApiResponse<string>.Fail("هذا المستخدم لا يملك دور ");
            if (roles.Contains("Admin"))
            {
                product.Status = status;
                _unitOfWork.Repository<Product>().Update(product);
                await _unitOfWork.CompleteAsync();
            }
            else if (roles.Contains("Farmer"))
            {
                if (status != ProductStatus.UnderReview && status != ProductStatus.Deleted)
                {
                    product.Status = status;
                    _unitOfWork.Repository<Product>().Update(product);
                    await _unitOfWork.CompleteAsync();
                }
                else
                    return ApiResponse<string>.Fail("لا يستطيع المزارع التغير لهذه الحالة");

            }
            else
                return ApiResponse<string>.Fail("هذا المستخدم لا يمكك صلاحية التغيير");

            return ApiResponse<string>.Ok(status.ToString(),"تم التحديث لهذه الحالة بنجاح");
        }

        public async Task<ApiResponse<CreateProductDto>> CreateAsync(string farmerId, CreateProductDto dto)
        {
            if (dto.UnitPrice <= 0 || dto.Quantity <= 20)
                return ApiResponse<CreateProductDto>.Fail("هناك قيود علي السعر والكمية");
            var farmer = await _userManager.FindByIdAsync(farmerId);
            if (farmer == null)
                return ApiResponse<CreateProductDto>.Fail("هذا المستخدم غير موجود");
            var roles = await _userManager.GetRolesAsync(farmer);
            if (!roles.Any() || !roles.Contains("Farmer") || !farmer.IsVerified || !farmer.IsActive)
                return ApiResponse<CreateProductDto>.Fail("هذا المستخدم ليس مزارع موثق");

            var category = await _unitOfWork.Repository<Category>().GetByIdAsync(dto.CategoryId);
            if (category is null)
                return ApiResponse<CreateProductDto>.Fail("هذه الفئة غير مدعومة");

            var product = _mapper.Map<Product>(dto);
            product.FarmerId = farmerId; 
            await _unitOfWork.Repository<Product>().AddAsync(product);
            await _unitOfWork.CompleteAsync();
            //save Images
            try
            {
                var firstImage = new ProductImage() { IsMain = true, ProductId = product.Id };
                for (int i = 0; i< dto.Images.Count(); i++)
                {
                    var url = await _imageService.SaveImageAsync(dto.Images[i], "products");
                    if (i == 0)
                    {
                        firstImage.ImageUrl = url;
                        await _unitOfWork.Repository<ProductImage>().AddAsync(firstImage);
                    }
                    else
                    {
                        var newImage = new ProductImage() { IsMain = false, ProductId = product.Id, ImageUrl = url };
                        await _unitOfWork.Repository<ProductImage>().AddAsync(newImage);
                    }
                    
                }                       
            }
            catch (Exception ex)
            {
                return ApiResponse<CreateProductDto>.Fail(ex.Message);
            }
           
            await _unitOfWork.CompleteAsync();
            return ApiResponse<CreateProductDto>.Ok(dto,"تمت إضافة المنتج بنجاح");
        }

        public async Task<ApiResponse<ProductSummaryDto>> UpdateAsync(string farmerId, int productId, UpdateProductDto dto)
        {
            var farmer = await _userManager.FindByIdAsync(farmerId);
            if (farmer == null)
                return ApiResponse<ProductSummaryDto>.Fail("هذا المستخدم غير موجود");
            var product = await _unitOfWork.Repository<Product>().GetByIdAsync(productId);
            if (product == null)
                return ApiResponse<ProductSummaryDto>.Fail("هذا المنتج غير موجود");
            if(product.Status == ProductStatus.SoldOut || product.Status == ProductStatus.Archived)
                return ApiResponse<ProductSummaryDto>.Fail("هذا المنتج  نفذ او لم يعد معروض");
            if (product.FarmerId != farmerId)
                return ApiResponse<ProductSummaryDto>.Fail("هذا المستخدم لا يملك صلاحية تعديل هذا المنتج");
            _mapper.Map(dto,product);
            product.HarvestDate = dto.HarvestDate ?? product.HarvestDate;
            product.ExpiryDate = dto.ExpiryDate ?? product.ExpiryDate;

            if (product.Quantity <= 0)
                product.Status = ProductStatus.SoldOut;
            _unitOfWork.Repository<Product>().Update(product);
            await _unitOfWork.CompleteAsync();
            var productSum = _mapper.Map<ProductSummaryDto>(product);
            return ApiResponse<ProductSummaryDto>.Ok(productSum,"تم تحديث البيانات بنجاح");
        }
        public async Task<ApiResponse<string>> DeleteAsync(string farmerId, int productId)
        {
            var farmer = await _userManager.FindByIdAsync(farmerId);
            if (farmer == null)
                return ApiResponse<string>.Fail("هذا المستخدم غير موجود");
            var product = await _unitOfWork.Repository<Product>().GetByIdAsync(productId);
            if (product == null)
                return ApiResponse<string>.Fail("هذا المنتج غير موجود");
            var roles = await _userManager.GetRolesAsync(farmer);
            if (!roles.Contains("Admin"))
            {
                if (product.FarmerId != farmerId)
                    return ApiResponse<string>.Fail("هذا المستخدم لا يملك صلاحية مسح هذا المنتج");
            }
            
            //hard delete
            // _unitOfWork.Repository<Product>().Delete(product);
            //await _unitOfWork.CompleteAsync();
            product.Status = ProductStatus.Deleted;
            _unitOfWork.Repository<Product>().Update(product);
            await _unitOfWork.CompleteAsync();
            return ApiResponse<string>.Ok("تم حذف المنتج بنجاح");
        }

        public async Task<ApiResponse<string>> AddImageAsync(string farmerId, int productId, IFormFile image)
        {
            var farmer = await _userManager.FindByIdAsync(farmerId);
            if (farmer == null)
                return ApiResponse<string>.Fail("هذا المستخدم غير موجود");
            var product = await _unitOfWork.Repository<Product>().GetByIdAsync(productId);
            if (product == null)
                return ApiResponse<string>.Fail("هذا المنتج غير موجود");
            if (product.FarmerId != farmerId)
                return ApiResponse<string>.Fail("هذا المستخدم لا يملك صلاحية تعديل هذا المنتج");

            var imageCountSpec = new BaseSpecifications<ProductImage>(pi => pi.ProductId == productId);
            var imgeCount = await _unitOfWork.Repository<ProductImage>().GetCountAsync(imageCountSpec);
            if (imgeCount >= 5)
                return ApiResponse<string>.Fail("لا يمكن إضافة أكثر من 5 صور للمنتج يمكنك حذف اي صورة ثم حاول مرة اخري");
            // Save new image
            var url = await _imageService.SaveImageAsync(image, "products");
            var newImage = new ProductImage() { IsMain = false, ProductId = productId, ImageUrl = url};
            await _unitOfWork.Repository<ProductImage>().AddAsync(newImage);
            await _unitOfWork.CompleteAsync();

            return ApiResponse<string>.Ok($"Image Id{newImage.Id.ToString()}", "تمت إضافة الصورة بنجاح");
        }
        public async Task<ApiResponse<string>> DeleteImageAsync(string farmerId, int productId, int imageId)
        {
            var farmer = await _userManager.FindByIdAsync(farmerId);
            if (farmer == null)
                return ApiResponse<string>.Fail("هذا المستخدم غير موجود");
            var product = await _unitOfWork.Repository<Product>().GetByIdAsync(productId);
            if (product == null)
                return ApiResponse<string>.Fail("هذا المنتج غير موجود");
            if (product.FarmerId != farmerId)
                return ApiResponse<string>.Fail("هذا المستخدم لا يملك صلاحية تعديل هذا المنتج");

            var imgSpec = new BaseSpecifications<ProductImage>(pi => pi.Id == imageId && pi.ProductId == productId && !pi.IsMain);
            var productImage = await _unitOfWork.Repository<ProductImage>().GetByIdWithSpecAsync(imgSpec); 
            if (productImage == null)
                return ApiResponse<string>.Fail("لا يمكن حذف صورة لاتخص المنتج ولا يمكن حذف الصورة الاساسية للمنتج ضع صورة اساسية اخري قبل الحذف");

            if (!string.IsNullOrEmpty(productImage.ImageUrl))
                _imageService.DeleteImage(productImage.ImageUrl);

            _unitOfWork.Repository<ProductImage>().Delete(productImage);
            await _unitOfWork.CompleteAsync();

            return ApiResponse<string>.Ok(imageId.ToString(),"تم حذف الصوررة بنجاج");
        }

        public async Task<ApiResponse<string>> SetMainImageAsync(string farmerId, int productId, int imageId)
        {
            var farmer = await _userManager.FindByIdAsync(farmerId);
            if (farmer == null)
                return ApiResponse<string>.Fail("هذا المستخدم غير موجود");
            var product = await _unitOfWork.Repository<Product>().GetByIdAsync(productId);
            if (product == null)
                return ApiResponse<string>.Fail("هذا المنتج غير موجود");
            if (product.FarmerId != farmerId)
                return ApiResponse<string>.Fail("هذا المستخدم لا يملك صلاحية تعديل هذا المنتج");

            var imgSpec = new BaseSpecifications<ProductImage>(pi => pi.ProductId == productId);
            var productImages = await _unitOfWork.Repository<ProductImage>().GetAllWithSpecAsync(imgSpec);
            if(!productImages.Select(pi => pi.Id).Contains(imageId))
                return ApiResponse<string>.Fail("هذه الصورة غير موجودة");     
            
            foreach (var image in productImages)
            {
                image.IsMain = false;
                _unitOfWork.Repository<ProductImage>().Update(image);
            }
            var newMainImage = productImages.FirstOrDefault(pi => pi.Id == imageId);
            newMainImage!.IsMain = true;
            _unitOfWork.Repository<ProductImage>().Update(newMainImage);
            await _unitOfWork.CompleteAsync();

            return ApiResponse<string>.Ok(imageId.ToString(),"تم ضبط هذه الصورة كصورة اساسية ");
        }
    }
}
