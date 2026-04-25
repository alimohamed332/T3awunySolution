using AutoMapper;
using AutoMapper.Execution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using T3awuny.Application.DTOs.Product;
using T3awuny.Core.Entities;

namespace T3awuny.Application.Helpers
{
    public class ProductNameResolver : IValueResolver<UpdateProductDto, Product, string>
    {
        public string Resolve(UpdateProductDto source, Product destination, string destMember, ResolutionContext context)
        {
            return source?.Name ?? destination.Name;
        }
    }
    public class ProductDescriptionResolver : IValueResolver<UpdateProductDto, Product, string?>
    {
        public string? Resolve(UpdateProductDto source, Product destination, string destMember, ResolutionContext context)
        {
            return source?.Description ?? destination.Description;
        }
    }
    public class ProductCategoryIdtResolver : IValueResolver<UpdateProductDto, Product, int>
    {
        public int Resolve(UpdateProductDto source, Product destination, int destMember, ResolutionContext context)
        {
            if (!source.CategoryId.HasValue || source.CategoryId.Value <= 0)
                return destination.CategoryId;

            return source.CategoryId.Value;
        }
    }

    public class ProductQuantityResolver : IValueResolver<UpdateProductDto, Product, decimal>
    {
        public decimal Resolve(UpdateProductDto source, Product destination, decimal destMember, ResolutionContext context)
        {
            if (!source.Quantity.HasValue || source.Quantity.Value <= 0)
                return destination.Quantity;

            return source.Quantity.Value;
        }
    }

    public class ProductUnitPriceResolver : IValueResolver<UpdateProductDto, Product, decimal>
    {
        public decimal Resolve(UpdateProductDto source, Product destination, decimal destMember, ResolutionContext context)
        {
            if (!source.UnitPrice.HasValue || source.UnitPrice.Value <= 0)
                return destination.UnitPrice;

            return source.UnitPrice.Value;
        }
    }

    public class ProductUnitResolver : IValueResolver<UpdateProductDto, Product, string>
    {
        public string Resolve(UpdateProductDto source, Product destination, string destMember, ResolutionContext context)
        {
            return source?.Unit ?? destination.Unit;
        }
    }

}
