using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using T3awuny.Core.Entities;

namespace T3awuny.Core.Specifications.ProductSpecs
{
    public class ProductSpecifications : BaseSpecifications<Product>
    {
        public ProductSpecifications(ProductSpecParams specs, bool AddInclude = false) : base
            ( P =>
               (string.IsNullOrEmpty(specs.Search) || P.Name.Contains(specs.Search)) && 
               (string.IsNullOrEmpty(specs.FarmerId) || P.FarmerId == specs.FarmerId) && 
               (!specs.CategoryId.HasValue || P.CategoryId == specs.CategoryId) &&
               (!specs.MinPrice.HasValue || P.UnitPrice >= specs.MinPrice) &&
               (!specs.MaxPrice.HasValue || P.UnitPrice <= specs.MaxPrice) &&
               (!specs.Status.HasValue || P.Status == specs.Status)
               
            )
        {
            if (AddInclude)
            {
                Includes.Add(p => p.Category);
                Includes.Add(p => p.Farmer);
                Includes.Add(p => p.Images);
            }

            if (!string.IsNullOrEmpty(specs.Sort))
            {
                switch (specs.Sort)
                {
                    case string sort when sort == "price" && !specs.SortDescending:
                        OrderBy = p => p.UnitPrice;
                        break;
                    case string sort when sort == "price" && specs.SortDescending :
                        OrderByDesc = p => p.UnitPrice;
                        break;
                    case string sort when sort == "date" && !specs.SortDescending:
                        OrderBy = p => p.HarvestDate!;
                        break;
                    case string sort when sort == "date" && specs.SortDescending:
                        OrderByDesc = p => p.HarvestDate!;
                        break;
                    default:
                        OrderBy = p => p.Name;
                        break;
                }
            }
            else
                OrderBy = p => p.Name;

            ApplyPagination((specs.PageIndex - 1) * specs.pageSize, specs.pageSize);

        }

        public ProductSpecifications(Expression<Func<Product,bool>> criteria, bool lighted = true) : base(criteria) 
        {
            if(lighted)
            {
                Includes.Add(p => p.Category);
                //Includes.Add(p => p.Images);
                Includes.Add(p => p.Images.Where(i => i.IsMain));
            }
            else
            {
                Includes.Add(p => p.Category);
                Includes.Add(p => p.Farmer);
                Includes.Add(p => p.Images);
            }
            
        }
    }
}
