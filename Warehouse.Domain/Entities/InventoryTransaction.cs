using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Warehouse.Domain.Entities
{
    public class InventoryTransaction
    {
        public string Id { get; set; }

        public string Type { get; set; }

        public string VariantId { get; set; }

        public string WarehouseId { get; set; }

        public int Quantity { get; set; }

        public string ReferenceType { get; set; }

        public string ReferenceId { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
