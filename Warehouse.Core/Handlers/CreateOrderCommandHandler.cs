using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Warehouse.Core.Commands;
using Warehouse.Core.Interfaces;
using Warehouse.Domain.Entities;

namespace Warehouse.Core.Handlers
{
    public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, string>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateOrderCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<string> Handle(CreateOrderCommand request, CancellationToken ct)
        {
            var dto = request.Model;
            string orderId = Guid.NewGuid().ToString();

            // Mở transaction trực tiếp từ UnitOfWork/DbContext
            await _unitOfWork.BeginTransactionAsync();
            string format = "dd/MM/yyyy HH:mm";
            try
            {
                // 1. Lấy số lượng đơn để tạo mã (Nên thực hiện trong transaction để tránh trùng mã)
                var currentOrderCount = await _unitOfWork.OrderRepository.CountAsync();
                int nextNumber = 1001 + currentOrderCount;

                // 2. Khởi tạo thực thể Order (Bảng chính)
                var order = new Orders
                {
                    Id = orderId,
                    OrderCode = "#" + nextNumber.ToString(),
                    CustomerId = dto.CustomerId,
                    SourceId = dto.SourceId,
                    BranchId = dto.BranchId,
                    StaffId = dto.StaffId,
                    Note = dto.Note,
                    OrderDate = DateTime.TryParseExact(dto.OrderDate, format,
                    System.Globalization.CultureInfo.InvariantCulture,
                    System.Globalization.DateTimeStyles.None,
                    out var od) ? od : null,
                    DeliveryDate = DateTime.TryParseExact(dto.DeliveryDate, format,
                    System.Globalization.CultureInfo.InvariantCulture,
                    System.Globalization.DateTimeStyles.None,
                    out var dd) ? dd : null,
                    TotalAmount = dto.TotalAmount,
                    Discount = dto.Discount,
                    ShippingFee = dto.ShippingFee,
                    FinalAmount = dto.FinalAmount,
                    PaymentStatus = dto.PaymentStatus,
                    PaymentMethod = dto.PaymentMethod,
                    PaidAmount = dto.PaidAmount,
                    ShippingMethod = dto.ShippingMethod,
                    FormDelivery = dto.FormDelivery,
                    CreatedAt = DateTime.Now,
                    CreatedBy = "admin"
                };
                await _unitOfWork.OrderRepository.AddAsync(order);

                // 3. Khởi tạo thực thể OrderShippingInfo (Bảng phụ - Quan hệ 1:1)
                var shippingInfo = new OrderShippingInfo
                {
                    Id = Guid.NewGuid().ToString(),
                    OrderId = orderId, // Sử dụng chung ID với Order chính
                    CodAmount = dto.CodAmount,
                    Weight = dto.Weight,
                    Length = dto.Length,
                    Width = dto.Width,
                    Height = dto.Height,
                    ShippingNote = dto.ShippingNote,
                    DeliveryRequirement = dto.DeliveryRequirement
                };
                await _unitOfWork.OrderShippingInfoRepository.AddAsync(shippingInfo);

                // 4. Xử lý chi tiết đơn hàng & Trừ kho
                if (dto.Items != null && dto.Items.Any())
                {
                    foreach (var itemDto in dto.Items)
                    {
                        var orderItem = new OrderItem
                        {
                            Id = Guid.NewGuid().ToString(),
                            OrderId = orderId,
                            ProductId = itemDto.ProductId,
                            Quantity = itemDto.Quantity,
                            UnitPrice = itemDto.UnitPrice,
                            TotalPrice = itemDto.Quantity * itemDto.UnitPrice
                        };
                        await _unitOfWork.OrderItemRepository.AddAsync(orderItem);

                        // Cập nhật tồn kho
                        var inventory = await _unitOfWork.InventoryRepository.Query()
                            .FirstOrDefaultAsync(x => x.ProductId == itemDto.ProductId);

                        if (inventory == null)
                            throw new Exception($"Sản phẩm {itemDto.ProductId} không tồn tại trong kho.");

                        if (inventory.StockQuantity < itemDto.Quantity)
                            throw new Exception($"Sản phẩm {itemDto.ProductId} không đủ tồn kho.");

                        inventory.StockQuantity -= itemDto.Quantity;
                        _unitOfWork.InventoryRepository.Update(inventory);
                    }
                }

                // 5. Đẩy dữ liệu xuống Database (Lệnh SaveChanges)
                await _unitOfWork.SaveAsync();

                // 6. CHỐT GIAO DỊCH (Nếu thiếu dòng này dữ liệu sẽ tự Rollback khi kết thúc hàm)
                await _unitOfWork.CommitTransactionAsync();

                return order.Id;
            }
            catch (Exception ex)
            {
                // Hoàn tác toàn bộ nếu có bất kỳ lỗi nào (Lỗi trừ kho, lỗi trùng mã đơn...)
                await _unitOfWork.RollbackTransactionAsync();
                throw new Exception("Lỗi tạo đơn hàng: " + ex.Message);
            }
        }
    
    
    }
}
