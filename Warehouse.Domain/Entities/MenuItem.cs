using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Warehouse.Domain.Entities
{
    public class MenuItem
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Path { get; set; }
        public string Icon { get; set; }
        public string? ParentId { get; set; }
        public int? SortOrder { get; set; }
        public bool IsActive { get; set; }
   
    }
}
