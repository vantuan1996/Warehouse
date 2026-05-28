using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Warehouse.Core.DTOs
{
    public class CategoryCreateDto
    {
        public string Name { get; set; }
        public string Description { get; set; } = string.Empty;
        public IFormFile Image { get; set; }
    }
    public class CategoryUpdateDto
    {
        //public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; } = string.Empty;
        public IFormFile? Image { get; set; }
    }
    public partial class CategoryDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public string ImageUrl { get; set; }

        public string CreatedAt { get; set; }


    }
    public class DeleteCategoryRequest
    {
        public List<string> Ids { get; set; }
    }
}
