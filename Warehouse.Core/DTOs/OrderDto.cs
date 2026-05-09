using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Warehouse.Core.DTOs
{
    public class OrderDto
    {
        public string CustomerId { get; set; }
        public string SourceId { get; set; }
        public string BranchId { get; set; }
        public string StaffId { get; set; }
        public string Note { get; set; }
        public string OrderDate { get; set; }
        public string DeliveryDate { get; set; }

        // Tài chính
        public decimal TotalAmount { get; set; }
        public decimal Discount { get; set; }
        public decimal ShippingFee { get; set; }
        public decimal FinalAmount { get; set; }
        public string PaymentStatus { get; set; }
        public string PaymentMethod { get; set; }
        public decimal PaidAmount { get; set; }

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
    public class OrderItemDto
    {
        public string ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
