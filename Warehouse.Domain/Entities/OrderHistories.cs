using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Warehouse.Domain.Entities
{
    public class OrderHistories
    {
        public string Id { get; set; }
        public string? OrderId { get; set; } 
        public string? ActorName { get; set; }
        public string? ActionType { get; set; }   // 'CREATE', 'CONFIRM', 'SHIPPING', 'PAYMENT', 'EMAIL'
        public string? Description { get; set; } 
        public string? Metadata { get; set; }
        public DateTime? CreatedAt { get; set; }

    }
}
