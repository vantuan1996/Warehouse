using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Warehouse.Core.DTOs
{
    public class InventoryDto
    {
        public string Id { get; set; }
        public string WarehouseName { get; set; }
        public string SumStock { get; set; } // tổng tồn kho của 1 sản phẩm trong 1 kho
        public string AvilableSell { get; set; } // số lượng có thể bán
        public string OnOrderQuantity { get; set; } //Đơn hàng mới tạo, đã giữ hàng nhưng chưa đóng gói.
        public string PackingQuantity { get; set; } //Đơn đã in vận đơn, đang đóng thùng.
        public string IncomingQuantity { get; set; }//Số lượng trên các đơn nhập hàng (Purchase Order) chưa về kho.

        public string UnsellableQuantity { get; set; } //Hàng hư hỏng, hàng trưng bày hoặc hàng đang đợi tiêu hủy.
    }
}
