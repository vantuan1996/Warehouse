using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Warehouse.Domain.Entities
{
    public class CustomerGroups
    {
        public string Id { get; set; }

        public string Name { get; set; } = null!;
        public string? Note { get; set; }
        public string? Type { get; set; }
    }
}
