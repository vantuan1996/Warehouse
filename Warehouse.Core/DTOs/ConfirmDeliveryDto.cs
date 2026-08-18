using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Warehouse.Core.DTOs
{
    public class ConfirmDeliveryDto
    {
        public string? DeliveryMethod { get; set; } // "store_pickup" hoặc "outside_shipper"
        public string? ShippingPartnerId { get; set; }
        public bool SendNotification { get; set; }
        public string? CancelReasonId { get; set; }
        public bool IsReturnToStock { get; set; }
        public string? RefundType { get; set; }
        public string? TypeOrder { get; set; }
    }
    public class ConfirmDeliveryResponse
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public string ExportDate { get; set; } // Trả về định dạng chuỗi "dd/MM/yyyy HH:mm" để hiển thị lên UI
    }
}
