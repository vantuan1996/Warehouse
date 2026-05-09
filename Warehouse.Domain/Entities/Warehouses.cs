using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Warehouse.Domain.Entities
{
    public class Warehouses
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Phone { get; set; }
        public bool IsDefault { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
