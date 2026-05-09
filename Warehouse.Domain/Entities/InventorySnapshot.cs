using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Warehouse.Domain.Entities
{
    public class InventorySnapshot
    {
        public string Id { get; set; }

        public string VariantId { get; set; }

        public string WarehouseId { get; set; }

        public int OnHand { get; set; }

        public int Available { get; set; }

        public int Reserved { get; set; }

        public int Incoming { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}
