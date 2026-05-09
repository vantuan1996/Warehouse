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

            // Bắt đầu Transaction để đảm bảo tính Atomic (Tất cả thành công hoặc không gì cả)
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                // 1. Lấy số lượng đơn hàng hiện tại để tạo mã tăng dần (#1001, #1002...)
                var currentOrderCount = await _unitOfWork.OrderRepository.CountAsync();
                int nextNumber = 1001 + currentOrderCount;

                // 2. Khởi tạo Entity Order
                var order = new Orders
                {
                    Id = orderId,
                    OrderCode = "#" + nextNumber.ToString(),
                    CustomerId = dto.CustomerId,
                    SourceId = dto.SourceId,
                    BranchId = dto.BranchId,
                    StaffId = dto.StaffId,
                    Note = dto.Note,
                    OrderDate = DateTime.TryParse(dto.OrderDate, out var od) ? od : DateTime.Now,
                    DeliveryDate = DateTime.TryParse(dto.DeliveryDate, out var dd) ? dd : null,

                    TotalAmount = dto.TotalAmount,
                    Discount = dto.Discount,
                    ShippingFee = dto.ShippingFee,
                    FinalAmount = dto.FinalAmount,
                    PaymentStatus = dto.PaymentStatus,
                    PaymentMethod = dto.PaymentMethod,
                    PaidAmount = dto.PaidAmount,

                    ShippingMethod = dto.ShippingMethod,
                    CodAmount = dto.CodAmount,
                    Weight = dto.Weight,
                    Length = dto.Length,
                    Width = dto.Width,
                    Height = dto.Height,
                    ShippingNote = dto.ShippingNote,
                    DeliveryRequirement = dto.DeliveryRequirement,

                    CreatedAt = DateTime.Now,
                    CreatedBy = "admin" // Có thể thay bằng User context thực tế
                };

                // 3. Xử lý Order Items và Cập nhật tồn kho
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

                        // Cập nhật tồn kho (Trừ kho)
                        var inventory = await _unitOfWork.InventoryRepository.Query()
                            .FirstOrDefaultAsync(x => x.ProductId == itemDto.ProductId);

                        if (inventory != null)
                        {
                            // Kiểm tra nếu tồn kho không đủ để trừ
                            if (inventory.StockQuantity < itemDto.Quantity)
                            {
                                throw new Exception($"Sản phẩm với ID {itemDto.ProductId} không đủ tồn kho để thực hiện giao dịch.");
                            }

                            inventory.StockQuantity -= itemDto.Quantity;
                            _unitOfWork.InventoryRepository.Update(inventory);
                        }
                    }
                }

                // 4. Lưu đơn hàng chính
                await _unitOfWork.OrderRepository.AddAsync(order);

                // Lưu tất cả thay đổi vào Database
                await _unitOfWork.SaveAsync();

                // Commit Transaction - Hoàn tất mọi thay đổi
                await _unitOfWork.CommitTransactionAsync();

                return order.Id;
            }
            catch (Exception ex)
            {
                // Nếu có bất kỳ lỗi nào xảy ra, hủy bỏ toàn bộ các thay đổi trước đó
                await _unitOfWork.RollbackTransactionAsync();
                throw new Exception("Lỗi hệ thống khi tạo đơn hàng: " + ex.Message);
            }
        }
    }
}
