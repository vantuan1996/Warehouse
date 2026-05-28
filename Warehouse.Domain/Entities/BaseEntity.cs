using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Warehouse.Domain.Entities
{
    public abstract class BaseEntity
    {
        public string Id { get; set; }
        public DateTime? CreatedAt { get; set; } = DateTime.Now;
        public string CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }
    }
    public abstract class BaseEntity2
    {
        public string Id { get; set; }
       
    }
}
