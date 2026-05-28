using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Warehouse.Domain.Entities
{
    public partial class Category
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public string ImageUrl { get; set; }

        public DateTime CreatedAt { get; set; }

    
    }
}
