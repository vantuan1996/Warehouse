using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Warehouse.Domain.Entities
{
    public class Product : BaseEntity
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public string? CodeSKU { get; set; }
        public string? BarCode { get; set; }
        public string? Unit { get; set; }

        public decimal? SellPrice { get; set; }
        public decimal? ComparativePrice { get; set; }
        public decimal? CapitalPrice { get; set; }

        public bool IsTax { get; set; }

        public string? CategoryId { get; set; }
      

        public string? BrandId { get; set; }
   

        //public ICollection<ProductImage> Images { get; set; } = new List<ProductImage>();
        //public ICollection<ProductVariant> Variants { get; set; } = new List<ProductVariant>();
        //public ICollection<Inventory> Inventories { get; set; } = new List<Inventory>();

        //public ProductSEO? SEO { get; set; }
        //public ProductShipping? Shipping { get; set; }
    }
}
