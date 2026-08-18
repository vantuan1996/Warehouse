using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Warehouse.Core.DTOs
{
    public class OrderHistoriesDto
    {
        public string Id { get; set; }
        public string OrderId { get; set; }
        public string ActionName { get; set; }
        public string ActionType { get; set; }   // 'CREATE', 'CONFIRM', 'SHIPPING', 'PAYMENT', 'EMAIL'
        public string Description { get; set; }
        public string Metadata { get; set; }
        public string CreatedAt { get; set; }
       
    }
}
