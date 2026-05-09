using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Warehouse.Domain.Entities
{
    public class Orders
    {
        public string Id { get; set; }
        public string OrderCode { get; set; } // Mã đơn hàng tự sinh
        public string CustomerId { get; set; }
        public string SourceId { get; set; } // Nguồn đơn
        public string BranchId { get; set; } // Chi nhánh
        public string StaffId { get; set; } // Nhân viên phụ trách
        public string Note { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime? DeliveryDate { get; set; }

        // Thanh toán
        public decimal TotalAmount { get; set; }
        public decimal Discount { get; set; }
        public decimal ShippingFee { get; set; }
        public decimal FinalAmount { get; set; }
        public string PaymentStatus { get; set; } // 'paid' hoặc 'later'
        public string PaymentMethod { get; set; } // Tiền mặt, chuyển khoản...
        public decimal PaidAmount { get; set; }

        // Giao hàng (Shipping)
        public string ShippingMethod { get; set; } // Cổng vận chuyển, tự giao...
        public decimal CodAmount { get; set; }
        public double Weight { get; set; }
        public double Length { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public string ShippingNote { get; set; }
        public string DeliveryRequirement { get; set; } // Cho xem hàng...

        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }


    }
}
