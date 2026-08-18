using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using Warehouse.Core.DTOs;
using Warehouse.Core.Interfaces;
using Warehouse.Core.Queries;
using Warehouse.Domain.Entities;

namespace Warehouse.Core.Handlers;

public class GetOrderHandler
   : IRequestHandler<GetOrderQuery, OrderDto?>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetOrderHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<OrderDto?> Handle(GetOrderQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // 1. Lấy thông tin bảng Order chính
            var order = await _unitOfWork.OrderRepository.GetByIdAsync(request.Id);
            if (order == null) return null;

            // 2. Lấy thông tin bảng phụ vận chuyển (Quan hệ 1:1 qua OrderId)
            var shippingInfo = await _unitOfWork.OrderShippingInfoRepository.Query()
                .FirstOrDefaultAsync(x => x.OrderId == order.Id, cancellationToken);

            // 3. Lấy danh sách sản phẩm thuộc đơn hàng (Quan hệ 1-N)
            var orderItems = await _unitOfWork.OrderItemRepository.Query()
                .Where(x => x.OrderId == order.Id)
                .ToListAsync(cancellationToken);

            // 4. Lấy lịch sử đơn hàng (Sắp xếp thời gian mới nhất lên đầu để hiển thị đúng trục Timeline)
            var orderHistories = await _unitOfWork.OrderHistoriesRepository.Query()
                .Where(x => x.OrderId == order.Id)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync(cancellationToken);

            // 5. Lấy thông tin khách hàng liên quan
            var customer = await _unitOfWork.CustomersRepository.GetByIdAsync(order.CustomerId);
            // 6. Lấy địa chỉ giao và nhận hàng
            // Thêm chữ Async vào sau FirstOrDefault
            var customerAdress = await _unitOfWork.CustomerAddressesRepository.Query()
                .FirstOrDefaultAsync(n => n.CustomerId == order.CustomerId && n.IsDefault, cancellationToken);
            //var source = await _unitOfWork.SourceRepository.GetByIdAsync(order.SourceId);
            //var branch = await _unitOfWork.BranchRepository.GetByIdAsync(order.BranchId);
            //var staff = await _unitOfWork.StaffRepository.GetByIdAsync(order.StaffId);

            // ĐÃ THÊM: Tính toán Tổng chi tiêu và Số lượng đơn hàng của Khách hàng này
            int totalOrdersCount = 0;
            decimal totalCustomerSpending = 0;
            var orderlastest = "";

            if (customer != null)
            {
                // Lấy tất cả đơn hàng của khách hàng này (Nên loại trừ các đơn trạng thái nháp hoặc đã hủy nếu cần, ví dụ: x.Status != "CANCEL")
                var allCustomerOrders = await _unitOfWork.OrderRepository.Query()
                    .Where(x => x.CustomerId == order.CustomerId && x.Status == "DELIVERED" )
                    .ToListAsync(cancellationToken);

                if (allCustomerOrders != null)
                {
                    totalOrdersCount = allCustomerOrders.Count;
                    totalCustomerSpending = allCustomerOrders.Where(x => x.StatusOrder != "CANCELLED").Sum(x => x.TotalAmount ); // Hoặc dùng x.TotalAmount tùy logic tính tiền của bạn
                    orderlastest = allCustomerOrders.Where(x => x.OrderDate != null).OrderByDescending(x => x.OrderDate).FirstOrDefault()?.OrderCode ?? "";
                }
            }
            // 6. Khởi tạo danh sách Items DTO đổ lên bảng sản phẩm
            var itemDtos = new List<OrderItemDto>();
            if (orderItems != null && orderItems.Any())
            {
                foreach (var item in orderItems)
                {
                    var product = await _unitOfWork.ProductsRepository.GetByIdAsync(item.ProductId);

                    itemDtos.Add(new OrderItemDto
                    {
                        ProductId = item.ProductId,
                        ProductName = product?.Name ?? "Sản phẩm không tồn tại",
                        Sku = product?.CodeSKU ?? "abc",
                        Quantity = item.Quantity,
                        UnitPrice = item.UnitPrice,
                        TotalPrice = item.TotalPrice
                    });
                }
            }

            // 7. Khởi tạo danh sách lịch sử đơn hàng DTO gửi về Frontend
            var historyDtos = new List<OrderHistoriesDto>();
            if (orderHistories != null && orderHistories.Any())
            {
                foreach (var history in orderHistories)
                {
                    historyDtos.Add(new OrderHistoriesDto
                    {
                        Id = history.Id,
                        ActionName = history.ActorName,     // Ví dụ: "Vũ Văn Tuấn" hoặc "Sapo"
                        ActionType = history.ActionType,   // Ví dụ: "CONFIRM", "PAYMENT", "EMAIL"
                        Description = history.Description, // Nội dung chuỗi text hiển thị
                        CreatedAt = history.CreatedAt?.ToString("dd/MM/yyyy HH:mm") ?? DateTime.Now.ToString("dd/MM/yyyy HH:mm") // Phục vụ nhóm theo ngày
                    });
                }
            }

            // 8. Tổ hợp toàn bộ dữ liệu trả về DTO khớp hoàn hảo với layout màn hình chi tiết đơn hàng
            var orderDto = new OrderDto
            {
                // Thông tin trạng thái gốc phục vụ ẩn/hiện nút giao hàng phía Client khi F5
                Status = order.Status,
              
                // Thông tin Khách hàng & Nguồn (Cột phải trên cùng)
                CustomerId = customer?.Id ?? "Chưa có tên",
                CustomerName = customer.FirstName + " " + customer.LastName,
                Email      = customer.Email,
                Mobile     = customer.Phone,
                Address    = customerAdress != null ? $"{customerAdress?.AddressLine}, {customerAdress?.Ward}, {customerAdress?.District}, {customerAdress?.Province}, {customerAdress?.Country}" : "Chưa có địa chỉ",
                PhoneAdress = customerAdress?.Phone,
                SourceId = "Admin",
                BranchId = "Cửa hàng chính",
                StaffId = "Vũ Văn Tuấn",
                OrderCode = order.OrderCode,
                //TotalMoneyOrder = CalSumOrder(),
                // Khung Ghi chú & Thời gian (Cột phải ở giữa/dưới)
                Note = order.Note,
                OrderDate = order.OrderDate?.ToString("dd/MM/yyyy HH:mm") ?? DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                DeliveryDate = order.DeliveryDate?.ToString("dd/MM/yyyy HH:mm") ?? "Chưa có ngày hẹn giao",
                TotalOrder = totalOrdersCount,
                TotalMoneyOrder = totalCustomerSpending,
                OrderLastest     = orderlastest,
                StatusOrder = order.StatusOrder,
                CancelDate = order.CancelledAt?.ToString("dd/MM/yyyy HH:mm"),
                // Khối Tài chính tính toán hiển thị
                TotalAmount = order.TotalAmount,
                Discount = order.Discount,
                ShippingFee = order.ShippingFee,
                FinalAmount = order.FinalAmount,
                PaymentStatus = order.PaymentStatus ?? "Chưa thanh toán",
                PaymentMethod = order.PaymentMethod ?? "Chưa chọn",
                PaidAmount = order.PaidAmount,

                // Khối dữ liệu Vận chuyển kết nối từ bảng OrderShippingInfo
                ShippingMethod = order.ShippingMethod,
                CodAmount = shippingInfo?.CodAmount ?? 0,
                Weight = shippingInfo?.Weight ?? 0,
                Length = shippingInfo?.Length ?? 0,
                Width = shippingInfo?.Width ?? 0,
                Height = shippingInfo?.Height ?? 0,
                ShippingNote = shippingInfo?.ShippingNote ?? "",
                DeliveryRequirement = shippingInfo?.DeliveryRequirement ?? "",
               

                // Danh sách sản phẩm hoàn thiện cấu trúc map
                Items = itemDtos,

                // Gán danh sách lịch sử đơn hàng vào DTO
                Histories = historyDtos
            };

            return orderDto;
        }
        catch (Exception ex)
        {

            throw ex;
        }
       
    }


}

public class GetAllOrderQueryHandler
  : IRequestHandler<GetAllOrderQuery, PagedResult<OrderDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetAllOrderQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }


    public async Task<PagedResult<OrderDto>> Handle(GetAllOrderQuery request, CancellationToken cancellationToken)
    {

        try
        {
            // 1. Khởi tạo Queryable từ OrderRepository của bạn giống như hàm Create
            var query = _unitOfWork.OrderRepository.Query();

            // 2. Xử lý tìm kiếm (Search) theo Mã đơn hàng hoặc các thông tin liên quan nếu có truyền vào
            if (!string.IsNullOrEmpty(request.Search))
            {
                var searchTerm = request.Search.Trim().ToLower();
                query = query.Where(x => x.OrderCode.ToLower().Contains(searchTerm) ||
                                         (x.Note != null && x.Note.ToLower().Contains(searchTerm)));
            }

            // 3. Tính tổng số lượng đơn hàng thỏa mãn điều kiện lọc (để tính tổng số trang hiển thị "Từ 1 đến 5 trên tổng 5")
            int totalItems = await query.CountAsync(cancellationToken);

            // 4. Thực hiện phân trang dựa vào Page và Limit từ Query bạn truyền vào
            // Đồng thời OrderByDescending theo ngày đặt để đơn mới nhất (#1005) luôn nhảy lên đầu bảng
            var pagedOrders = await query
                .OrderByDescending(x => x.OrderDate)
                .Skip((request.Page - 1) * request.Limit)
                .Take(request.Limit)
                .ToListAsync(cancellationToken);

            // 5. Duyệt qua danh sách đơn hàng đã phân trang để bốc thông tin Text từ các Repo khác (Khách hàng, Nguồn đơn)
            var orderDtos = new List<OrderDto>();

            foreach (var order in pagedOrders)
            {
                // Lấy tên khách hàng từ CustomerRepository dựa vào CustomerId
                var customer = !string.IsNullOrEmpty(order.CustomerId)
                    ? await _unitOfWork.CustomersRepository.GetByIdAsync(order.CustomerId)
                    : null;
                var addressCus = !string.IsNullOrEmpty(order.CustomerId)
              ? await _unitOfWork.CustomerAddressesRepository
                  .Query()
                  .Where(x => x.CustomerId == order.CustomerId)
                  .Select(x => new
                  {
                      x.Id,
                      x.Ward,
                      x.District,
                      x.Province,
                      x.AddressLine,
                      x.Country
                  })
                  .FirstOrDefaultAsync()
                    : null;
                // Lấy tên nguồn đơn (Admin, Facebook, Shopee...) từ SourceRepository dựa vào SourceId
                //var source = !string.IsNullOrEmpty(order.SourceId)
                //    ? await _unitOfWork.SourceRepository.GetByIdAsync(order.SourceId)
                //    : null;
                var itemDtos = new List<OrderItemDto>();
                var orderItems = await _unitOfWork.OrderItemRepository.Query()
              .Where(x => x.OrderId == order.Id)
              .ToListAsync(cancellationToken);


                // 5. Khởi tạo danh sách Items DTO đổ lên bảng sản phẩm (Bên trái giao diện)

                if (orderItems != null && orderItems.Any())
                {
                    foreach (var item in orderItems)
                    {
                        // Lấy thông tin sản phẩm (để lấy Tên sản phẩm, Mã SKU hiển thị lên UI như "bạc 1 | đơn vị: cái")
                        var product = await _unitOfWork.ProductsRepository.GetByIdAsync(item.ProductId);

                        itemDtos.Add(new OrderItemDto
                        {
                            ProductId = item.ProductId,

                            ProductName = product?.Name ?? "Sản phẩm không tồn tại", // Tên hiển thị ("bạc 1")
                            Sku = product?.CodeSKU ?? "abc",                           // Mã SKU hiển thị kèm theo
                            Quantity = item.Quantity,                              // Số lượng ("1")
                            UnitPrice = item.UnitPrice,                                // Đơn giá ("1,000đ")
                            TotalPrice = item.TotalPrice                                // Thành tiền ("1,000đ")
                        });
                    }
                }
                // Map toàn bộ dữ liệu ra DTO phẳng để UI chỉ việc đem đi hiển thị
                orderDtos.Add(new OrderDto
                {
                    Id = order.Id,
                    OrderCode = order.OrderCode ?? "",
                    OrderDate = order.OrderDate?.ToString("dd/MM/yyyy HH:mm") ?? "---",

                    // KIỂM TRA XOÁ MÙ: Nếu không có khách hàng (null) thì hiển thị gạch ngang "---" y hệt dòng đơn #1005 và #1003 trong ảnh
                    CustomerName = customer?.LastName ?? "---",
                    Mobile = customer?.Phone ?? "---",
                    Email = customer?.Email ?? "---",
                    Address = addressCus?.Ward + ", " + addressCus?.District + ", " + addressCus?.Province + ", " + addressCus?.Country,
                    // Nếu nguồn đơn trống thì mặc định hiển thị Admin như ảnh của bạn
                    SourceName = /*source?.Name ?? */"Admin",
                    StatusOrder = order.StatusOrder,
                    FinalAmount = order.FinalAmount,       // Số tiền cột "Thành tiền"
                    PaymentStatus = order.PaymentStatus ?? "Chưa thanh toán", // Tag trạng thái thanh toán
                    Status = order.Status, // Tag trạng thái xử lý
                    ShippingMethod = order.ShippingMethod ?? "", // Cột dịch vụ vận chuyển (nếu có)
                    Items = itemDtos
                });
            }



            return new PagedResult<OrderDto>
            {
                Items = orderDtos.OrderBy(n => n.OrderCode),
                Total = totalItems
            };
        }
        catch (Exception ex)
        {

            throw ex;
        }
       
    }
}

