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
        public DateTime? OrderDate { get; set; }
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
        public int? FormDelivery { get; set; } // 1 là Khách nhanạ tại cửa hàng 2 đối tác vận chuyển ngoài
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public string? Status { get; set; }// confirm xác định giao hàng
        public string? CancelReasonId { get; set; } // ID lý do hủy (để map với mảng/bảng danh mục lý do hủy)

        //public string? CancelNote { get; set; } // Chi tiết lý do (nếu chọn 'Lý do khác' và nhập text)

        public DateTime? CancelledAt { get; set; } // Ngày giờ hủy đơn hàng
        public string? StatusOrder { get; set; } // từ chối , hủy , trả hàng, đổi trả hàng....
        public string? CancelledBy { get; set; } // ID người hủy (Nhân viên, hoặc 'SYSTEM', hoặc 'CUSTOMER')

        // --- XỬ LÝ TIỀN & KHO KHI HỦY ---

        public string? CancelRefundType { get; set; } // Hình thức hoàn trả: 'refund' (Hoàn tiền ngay) hoặc 'refund_later' (Hoàn trả sau)

        public DateTime? ExportDateOrder { get; set; } // ngày xuât hàng


    }


}
