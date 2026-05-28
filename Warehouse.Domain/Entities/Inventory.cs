using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Warehouse.Domain.Entities
{

    [Table("Inventory")]
    public class Inventory :BaseEntity
    {

        public string? ProductId { get; set; }
        public string? WarehouseId { get; set; }
        public string? VariantId { get; set; }
        public int StockQuantity { get; set; } //-- 1. Tồn kho (Physical Stock): Tổng số lượng thực tế trong kho
        public int OnOrderQuantity { get; set; }//-- 2. Đang giao dịch (On Orders): Khách đã đặt nhưng chưa xử lý/đóng gói
        public int PackingQuantity { get; set; }//-- 3. Đang đóng gói (Packing): Đang trong quá trình đóng gói/chờ shipper lấy
        public int IncomingQuantity { get; set; }//-- 4. Hàng đang về (Incoming): Đang nhập hàng từ NCC, chưa vào kho
        public int UnsellableQuantity { get; set; }//-- 5. Không thể bán (Unsellable/Damaged): Hàng lỗi, hàng hoàn chờ kiểm định

        public DateTime? LastUpdated { get; set; }

       
    }
}
