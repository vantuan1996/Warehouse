using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Warehouse.Core.DTOs;
using Warehouse.Core.Interfaces;
using Warehouse.Core.Queries;
using Warehouse.Domain.Entities;

namespace Warehouse.Core.Handlers
{
    public class GetProductQueryHandler
    : IRequestHandler<GetProductQuery, ProductDto?>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetProductQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ProductDto?> Handle(GetProductQuery request, CancellationToken cancellationToken)
        {
            // 1. Lấy thông tin sản phẩm cơ bản
            var product = await _unitOfWork.ProductsRepository.Query()
                .Where(p => p.Id == request.Id)
                .Select(p => new
                {
                    p.Id,
                    p.Name,
                    p.Description,
                    p.CodeSKU,
                    p.BarCode,
                    p.Unit,
                    p.SellPrice,
                    p.CategoryId
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (product == null) return null;

            // 2. Xử lý danh mục (Categories)
            var categoryIds = product.CategoryId?
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .ToList() ?? new List<string>();

            var categories = await _unitOfWork.CategoryRepository.Query()
                .Where(n => categoryIds.Contains(n.Id.ToString()))
                .Select(n => new CategoryDto
                {
                    Id = n.Id,
                    Name = n.Name
                })
                .ToListAsync(cancellationToken);

            // 3. Xử lý hình ảnh
            var images = await _unitOfWork.ImagesRepository.Query()
                .Where(img => img.FkId == product.Id)
                .OrderByDescending(img => img.IsMain)
                .Select(img => new ObjFiles
                {
                    Url = img.ImageUrl,
                    IsMain = img.IsMain
                })
                .ToListAsync(cancellationToken);
   
            // 4. Xử lý tồn kho (Inventory)
            var inventory = new List<InventoryDto>();
            try
            {
                 inventory = await _unitOfWork.InventoryRepository.Query()
                .Where(n => n.ProductId == product.Id)
                .Select(n => new InventoryDto
                {
                    Id = n.Id,
                    WarehouseName = "Oki", // Thêm tên kho để hiển thị lên UI
                    SumStock = n.StockQuantity.ToString(),

                    // Tính toán số lượng có thể bán: Tồn - (Giao dịch + Đóng gói + Không thể bán)
                    AvilableSell = (n.StockQuantity - (n.OnOrderQuantity + n.PackingQuantity + n.UnsellableQuantity)).ToString(),

                    OnOrderQuantity = n.OnOrderQuantity.ToString(),
                    PackingQuantity = n.PackingQuantity.ToString(),
                    IncomingQuantity = n.IncomingQuantity.ToString(),
                    UnsellableQuantity = n.UnsellableQuantity.ToString()
                })
                .ToListAsync(cancellationToken);
            }
            catch (Exception ex)
            {

                throw;
            }
            

            // 5. Trả về kết quả cuối cùng
            var result = new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                CodeSKU = product.CodeSKU,
                BarCode = product.BarCode,
                Unit = product.Unit,

                // Ép kiểu an toàn
                SellPrice = product.SellPrice ?? 0,
                ComparativePrice = product.SellPrice ?? 0,
                CapitalPrice = product.SellPrice ?? 0,

                cates = categories,
                objImages = images,
                lstInvent = inventory
            };

            return result;
        }


        public class GetAllProductQueryHandler
        : IRequestHandler<GetALLProductQuery, PagedResult<ProductDto>>
        {
            private readonly IUnitOfWork _unitOfWork;

            public GetAllProductQueryHandler(IUnitOfWork unitOfWork)
            {
                _unitOfWork = unitOfWork;
            }

            public async Task<PagedResult<ProductDto>> Handle(GetALLProductQuery request, CancellationToken cancellationToken)
            {
                var query = from p in _unitOfWork.ProductsRepository.Query()

                                // Join với kho hàng để lấy tồn kho (giữ nguyên logic của bạn)
                            join inv in _unitOfWork.InventoryRepository.Query()
                                on p.Id equals inv.ProductId into invs

                            where string.IsNullOrEmpty(request.Search) || p.Name.Contains(request.Search)

                            select new ProductDto
                            {
                                Id = p.Id,
                                Name = p.Name,
                                BarCode = p.BarCode.ToString(),
                                CodeSKU = p.CodeSKU.ToString(),
                                Unit  = p.Unit.ToString(),
                                SellPrice = p.SellPrice ?? 0,
                                // 🖼️ LẤY DANH SÁCH ẢNH VÀ TRẠNG THÁI ISMAIN
                                objImages = _unitOfWork.ImagesRepository.Query()
                                            .Where(img => img.FkId == p.Id)
                                            .OrderByDescending(img => img.IsMain) // IsMain = true (1) sẽ lên đầu
                                            .Select(img => new ObjFiles
                                            {
                                                Url = img.ImageUrl,
                                                IsMain = img.IsMain
                                            })
                                            .ToList(),
                                urlMain = _unitOfWork.ImagesRepository.Query()
                                            .Where(img => img.FkId == p.Id && img.IsMain == true)
                                            .FirstOrDefault().ImageUrl,

                                AvailableQuantity = invs.Sum(x => (int?)(x.StockQuantity - (x.OnOrderQuantity + x.PackingQuantity + x.UnsellableQuantity))) ?? 0,
                                CreatedAt = p.CreatedAt.HasValue
                                ? p.CreatedAt.Value.ToString("dd/MM/yyyy")
                : string.Empty
                            };

                // Phân trang
                var items = await query
                    .Skip((request.Page - 1) * request.Limit)
                    .Take(request.Limit)
                    .ToListAsync(cancellationToken);

                // Tính tổng số lượng để phân trang ở Frontend
                var totalCount = await _unitOfWork.ProductsRepository.Query()
                    .CountAsync(x => string.IsNullOrEmpty(request.Search) || x.Name.Contains(request.Search), cancellationToken);

                return new PagedResult<ProductDto>
                {
                    Items = items,
                    Total = totalCount
                };
            }
        }
    }


}
