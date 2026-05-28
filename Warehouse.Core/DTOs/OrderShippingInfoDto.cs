using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Warehouse.Core.DTOs
{
    public class OrderShippingInfoDto
    {
       
        // Vận chuyển
        public string ShippingMethod { get; set; }
        public decimal CodAmount { get; set; }
        public double Weight { get; set; }
        public double Length { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public string ShippingNote { get; set; }
        public string DeliveryRequirement { get; set; }

        // Danh sách sản phẩm
        public List<OrderItemDto> Items { get; set; }
    }
   
}
