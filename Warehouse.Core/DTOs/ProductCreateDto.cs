using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Warehouse.Domain.Entities;

namespace Warehouse.Core.DTOs
{
    public class ProductCreateDto
    {
        public string Name { get; set; }
        public string Description { get; set; } = string.Empty;
        public string CodeSKU { get; set; } = string.Empty;
        public string BarCode { get; set; } = string.Empty;
        public string UnitCal { get; set; } = string.Empty;
        public string SellPrice { get; set; } = string.Empty;
        public string ComparativePrice { get; set; } = string.Empty;
        public string CapitalPrice { get; set; } = string.Empty;
        public bool isTax { get; set; }
        public string weight { get; set; }
        public string selectedCategories { get; set; } // Nhận dưới dạng chuỗi JSON
        public string BrandId { get; set; }
        public string Stock { get; set; }

        // SỬA Ở ĐÂY: Thêm List hoặc mảng để nhận nhiều file

        //public List<IFormFile> Images { get; set; } = new List<IFormFile>();
        //public List<ObjFiles> Images { get; set; } = new List<ObjFiles>();
        public List<IFormFile> Images { get; set; } = new List<IFormFile>();

        // Hứng chuỗi JSON metadata (isMain, url...)
        public string ImagesMetadata { get; set; }
    }
    public class ObjFiles
    {
        //public string Id { get; set; }
        public string FileName { get; set; }
        public bool IsMain { get; set; }
        public string Url { get; set; }
    }
    public class ProductUpdateDto
    {
        //public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; } = string.Empty;
        public IFormFile? Image { get; set; }
    }
    public partial class   ProductDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string CodeSKU { get; set; }
        public string BarCode { get; set; }
        public string Unit { get; set; }
        public decimal SellPrice { get; set; }
        public decimal ComparativePrice { get; set; }
        public decimal CapitalPrice { get; set; }
        public bool IsTax { get; set; }
        public string CategoryId { get; set; }
        public Category Category { get; set; }

        public string? BrandId { get; set; }

        public List<ObjFiles> objImages { get; set; }
        public string urlMain { get; set; }
        public int AvailableQuantity { get; set; }
        public string BrandName { get; set; }
        public List<CategoryDto> cates { get; set; }
        public string ProductType { get; set; }

        public string CreatedAt { get; set; }
        public List<InventoryDto> lstInvent { get; set; }
        //public InventoryDto objInvent { get; set; }


    }
    //public class DeleteCategoryRequest
    //{
    //    public List<string> Ids { get; set; }
    //}
}
