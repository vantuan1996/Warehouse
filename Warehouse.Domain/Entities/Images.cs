using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Warehouse.Domain.Entities
{
    public class Images :BaseEntity
    {
        public string ImageUrl { get; set; }
        public bool IsMain { get; set; }

        public string FkId { get; set; } /// cột để join giua các bảng
      
    }
}
