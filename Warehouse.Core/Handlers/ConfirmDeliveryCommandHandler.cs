using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Warehouse.Core.Commands;
using Warehouse.Core.DTOs;
using Warehouse.Core.Interfaces;
using Warehouse.Domain.Entities;

namespace Warehouse.Core.Handlers
{
    public class ConfirmDeliveryCommandHandler : IRequestHandler<ConfirmDeliveryCommand, ConfirmDeliveryResponse>
    {
        private readonly IUnitOfWork _unitOfWork;

        public ConfirmDeliveryCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<ConfirmDeliveryResponse> Handle(ConfirmDeliveryCommand request, CancellationToken ct)
        {
            var dto = request.Model;

            // Mở transaction trực tiếp từ UnitOfWork giống hàm tạo đơn
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                if (dto.TypeOrder == "CANCELORDER")
                {
                    // 1. Tìm đơn hàng cần xử lý hủy
                    var orderToCancel = await _unitOfWork.OrderRepository.Query()
                        .FirstOrDefaultAsync(x => x.Id == request.OrderId.ToString(), ct);

                    if (orderToCancel == null)
                        throw new Exception($"Đơn hàng có ID {request.OrderId} không tồn tại.");

                    // Kiểm tra nếu đơn hàng đã bị hủy trước đó rồi thì không xử lý lại
                    if (orderToCancel.StatusOrder == "CANCELLED")
                        throw new Exception("Đơn hàng này đã được hủy từ trước.");

                    DateTime currentCancelTime = DateTime.Now;
                    

                    // 2. Cập nhật các thông tin liên quan đến hủy đơn hàng (Tránh ghi đè Status chính)
                    orderToCancel.StatusOrder = "CANCELLED"; // Đánh dấu trạng thái hủy độc lập
                    orderToCancel.CancelReasonId = dto.CancelReasonId;
                    orderToCancel.CancelRefundType = dto.RefundType;
                    orderToCancel.CancelledAt = currentCancelTime;
                    orderToCancel.CancelledBy = "Vũ Văn Tuấn";
                    // Lưu các thông tin từ client truyền xuống (mapping từ DTO của bạn)
                    //// Giả sử DTO của bạn chứa các trường tương ứng từ Form UI: CancelReasonId, CancelRefundType, IsReturnedToStock
                    //orderToCancel.Note = !string.IsNullOrEmpty(dto.CancelReasonNote)
                    //    ? $"[Hủy đơn] {dto.CancelReasonNote}"
                    //    : orderToCancel.Note;

                    _unitOfWork.OrderRepository.Update(orderToCancel);

                    // 3. LOG LỊCH SỬ HỦY ĐƠN: Thêm thông tin vào bảng OrderHistories
                    string refundTypeText = dto.RefundType == "refund" ? "Hoàn tiền ngay" : "Hoàn trả sau";
                    string stockReturnText = dto.IsReturnToStock ? "Có hoàn kho" : "Không hoàn kho";
                    string cancelReasonText = dto.CancelReasonId;

                    var historyCancel = new OrderHistories
                    {
                        Id = Guid.NewGuid().ToString(),
                        OrderId = orderToCancel.Id,
                        ActorName = "VŨ VĂN TUẤN",
                        ActionType = "CANCEL",
                        Description = $"Đã hủy đơn hàng. Lý do: {cancelReasonText}. Hình thức: {refundTypeText}. Trạng thái kho: {stockReturnText}.",
                        CreatedAt = currentCancelTime
                    };
                    await _unitOfWork.OrderHistoriesRepository.AddAsync(historyCancel);
                    // --- PHẦN XỬ LÝ HOÀN TIỀN (BỔ SUNG THEO IMAGE_C0CD83.PNG) ---
                    // Chỉ tạo log hoàn tiền nếu đơn này khách đã từng thanh toán (PaidAmount > 0) và chọn "Hoàn tiền ngay"
                    if (dto.RefundType == "refund" && orderToCancel.PaidAmount > 0)
                    {
                        string paymentMethodText = "Chuyển khoản";
                        if (orderToCancel.PaymentMethod?.ToLower() == "cash" || orderToCancel.PaymentMethod == "Tiền mặt")
                        {
                            paymentMethodText = "Tiền mặt";
                        }

                        var historyMoneyRefund = new OrderHistories
                        {
                            Id = Guid.NewGuid().ToString(),
                            OrderId = orderToCancel.Id,
                            ActorName = "VŨ VĂN TUẤN",
                            ActionType = "REFUND_MONEY", // Đặt ActionType riêng để phân biệt luồng tài chính
                            Description = $"Đã hoàn lại {orderToCancel.PaidAmount:N0} VND thông qua {paymentMethodText}",
                            CreatedAt = currentCancelTime
                        };
                        await _unitOfWork.OrderHistoriesRepository.AddAsync(historyMoneyRefund);

                        // Mở rộng sau này: Nếu bạn có cột Số tiền thực tế còn lại của đơn sau khi hoàn, bạn có thể cập nhật orderToCancel.PaidAmount = 0 tại đây.
                    }
                    // 4. XỬ LÝ HOÀN KHO (INVENTORY): Chuẩn hóa theo bảng Inventory của bạn
                    if (dto.IsReturnToStock)
                    {
                        // Lấy danh sách sản phẩm trong đơn hàng
                        var orderItems = await _unitOfWork.OrderItemRepository.Query()
                            .Where(x => x.OrderId == orderToCancel.Id)
                            .ToListAsync(ct);

                        foreach (var item in orderItems)
                        {
                            // Tìm bản ghi kho dựa theo ProductId và WarehouseId (mặc định là "DEFAULT" như trong DB của bạn)
                            var inventory = await _unitOfWork.InventoryRepository.Query()
                                .FirstOrDefaultAsync(x => x.ProductId == item.ProductId && x.WarehouseId == "DEFAULT", ct);

                            if (inventory != null)
                            {
                                // Tăng số lượng tồn kho thực tế lên
                                inventory.StockQuantity += item.Quantity;

                                // Nếu hệ thống của bạn có trừ/cộng vào hàng đang đặt khi tạo đơn, hãy hạ nó xuống tại đây:
                                if (inventory.OnOrderQuantity >= item.Quantity)
                                {
                                    inventory.OnOrderQuantity -= item.Quantity;
                                }

                                inventory.LastUpdated = currentCancelTime; // Cập nhật ngày thay đổi tồn kho

                                _unitOfWork.InventoryRepository.Update(inventory);
                            }
                        }

                        var historyCancel2 = new OrderHistories
                        {
                            Id = Guid.NewGuid().ToString(),
                            OrderId = orderToCancel.Id,
                            ActorName = "VŨ VĂN TUẤN",
                            ActionType = "CANCEL",
                            Description = $"Đã nhập kho  {orderItems.Sum(x => x.Quantity)} sản phẩm tại 1 chi nhánh",
                            CreatedAt = currentCancelTime
                        };
                        await _unitOfWork.OrderHistoriesRepository.AddAsync(historyCancel2);
                    }
                    var historyMoney1 = new OrderHistories
                    {
                        Id = Guid.NewGuid().ToString(),
                        OrderId = orderToCancel.Id,
                        ActorName = "VŨ VĂN TUẤN",
                        ActionType = "CANCEL", // Đặt ActionType riêng để phân biệt luồng tài chính
                        Description = $"Đã hủy đơn hàng",
                        CreatedAt = currentCancelTime
                    };
                    await _unitOfWork.OrderHistoriesRepository.AddAsync(historyMoney1);

                    var historyEmail3 = new OrderHistories
                    {
                        Id = Guid.NewGuid().ToString(),
                        OrderId = orderToCancel.Id,
                        ActorName = "SystemMail",
                        ActionType = "EMAIL",
                        Description = "Thông báo hủy đơn hàng đã được gửi tới khách hàng",
                        CreatedAt = DateTime.Now
                    };
                    await _unitOfWork.OrderHistoriesRepository.AddAsync(historyEmail3);
                    // 5. Lưu toàn bộ thay đổi và commit transaction phục vụ riêng cho luồng Hủy đơn
                    await _unitOfWork.SaveAsync();
                    await _unitOfWork.CommitTransactionAsync();

                    // Trả về kết quả sớm, kết thúc luồng xử lý không để chạy xuống khối DELIVERED phía dưới
                    return new ConfirmDeliveryResponse
                    {
                        IsSuccess = true,
                        Message = "Hủy đơn hàng thành công",
                        ExportDate = currentCancelTime.ToString("dd/MM/yyyy HH:mm")
                    };
                }
                else
                {
                    // 1. Tìm đơn hàng cần xử lý
                    var order = await _unitOfWork.OrderRepository.Query()
                        .FirstOrDefaultAsync(x => x.Id == request.OrderId.ToString(), ct);

                    if (order == null)
                        throw new Exception($"Đơn hàng có ID {request.OrderId} không tồn tại.");

                    // Lấy thời gian hiện tại lúc bấm nút xác nhận
                    DateTime currentExportTime = DateTime.Now;

                    // Cập nhật trạng thái đơn hàng chính thành Đã giao hàng
                    order.Status = "DELIVERED";
                    order.DeliveryDate = currentExportTime;
                    // Cập nhật Ngày xuất hàng trực tiếp vào đơn hàng để hiển thị ở khối "Đã xử lý giao hàng" giống ảnh bạn khoanh
                    order.DeliveryDate = currentExportTime; // Hoặc order.ExportDate tùy theo tên cột trong DB của bạn

                    _unitOfWork.OrderRepository.Update(order);

                    string actorName = "Vũ Văn Tuấn";

                    // Truy vấn lấy tên khách hàng (Ghép FirstName và LastName)
                    string customerName = await _unitOfWork.CustomersRepository.Query()
                        .Where(c => c.Id == order.CustomerId)
                        .Select(c => c.FirstName + " " + c.LastName)
                        .FirstOrDefaultAsync(ct) ?? "Khách hàng";

                    // 2. LOG 1: Xác nhận đơn hàng
                    var historyConfirm = new OrderHistories
                    {
                        Id = Guid.NewGuid().ToString(),
                        OrderId = order.Id,
                        ActorName = actorName,
                        ActionType = "CONFIRM",
                        Description = $"Đã xác nhận đơn hàng từ {customerName.ToLower()}",
                        CreatedAt = currentExportTime
                    };
                    await _unitOfWork.OrderHistoriesRepository.AddAsync(historyConfirm);

                    // 3. LOG 2: Log xác nhận khoản thanh toán tài chính
                    if (order.PaidAmount > 0)
                    {
                        string paymentMethodText = "Chuyển khoản";
                        if (order.PaymentMethod?.ToLower() == "cash" || order.PaymentMethod == "Tiền mặt")
                        {
                            paymentMethodText = "Tiền mặt";
                        }

                        var historyPayment = new OrderHistories
                        {
                            Id = Guid.NewGuid().ToString(),
                            OrderId = order.Id,
                            ActorName = actorName,
                            ActionType = "PAYMENT",
                            Description = $"Đã xác nhận khoản thanh toán {order.PaidAmount:N0} VND thông qua {paymentMethodText}",
                            CreatedAt = currentExportTime
                        };
                        await _unitOfWork.OrderHistoriesRepository.AddAsync(historyPayment);
                    }

                    // 4. LOG 3: Hình thức vận chuyển
                    string deliveryDesc = "";
                    if (dto.DeliveryMethod == "pick_up")
                    {
                        deliveryDesc = "Khách nhận tại cửa hàng";
                    }
                    else if (dto.DeliveryMethod == "outside_shipper")
                    {
                        //var partner = await _unitOfWork.BrandsRepository.Query()
                        //    .FirstOrDefaultAsync(x => x.Id == dto.ShippingPartnerId.ToString(), ct);

                        //string partnerName = partner != null ? partner.Name : "Đối tác khác";
                        //deliveryDesc = $"Đã chuyển giao đơn hàng cho đối tác {partnerName}";
                    }

                    if (!string.IsNullOrEmpty(deliveryDesc))
                    {
                        var historyShipping = new OrderHistories
                        {
                            Id = Guid.NewGuid().ToString(),
                            OrderId = order.Id,
                            ActorName = actorName,
                            ActionType = "SHIPPING",
                            Description = deliveryDesc,
                            CreatedAt = currentExportTime
                        };
                        await _unitOfWork.OrderHistoriesRepository.AddAsync(historyShipping);
                    }

                    // 5. LOG 4: Bắn email tự động thông báo cho khách hàng
                    if (dto.SendNotification)
                    {
                        var historyEmail = new OrderHistories
                        {
                            Id = Guid.NewGuid().ToString(),
                            OrderId = order.Id,
                            ActorName = "SystemMail",
                            ActionType = "EMAIL",
                            Description = "Email xác nhận đơn hàng đã được gửi tới khách hàng",
                            CreatedAt = currentExportTime
                        };
                        await _unitOfWork.OrderHistoriesRepository.AddAsync(historyEmail);
                    }

                    // 6. Đẩy toàn bộ dữ liệu xuống database tập trung
                    await _unitOfWork.SaveAsync();

                    // 7. Chốt giao dịch thành công (Commit transaction)
                    await _unitOfWork.CommitTransactionAsync();

                    // Trả về dữ liệu kết quả kèm thời gian định dạng đúng chuẩn "dd/MM/yyyy HH:mm" như trên ảnh
                    return new ConfirmDeliveryResponse
                    {
                        IsSuccess = true,
                        Message = "Đã gửi thành công",
                        ExportDate = currentExportTime.ToString("dd/MM/yyyy HH:mm")
                    };
                }
               
            }
            catch (Exception ex)
            {
                // Hoàn tác dữ liệu ngay lập tức nếu lỗi
                await _unitOfWork.RollbackTransactionAsync();
                throw new Exception("Lỗi khi xác nhận giao hàng: " + ex.Message);
            }
        }
   
    
    }


}
