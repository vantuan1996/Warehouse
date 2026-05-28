using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Warehouse.Domain.Entities
{
    public class OrderShippingInfo
    {
        public string Id { get; set; }
        public string OrderId { get; set; }
        public decimal CodAmount { get; set; }
        public double Weight { get; set; }
        public double Length { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public string ShippingNote { get; set; }
        public string DeliveryRequirement { get; set; } // Cho xem hàng...


    }
}
