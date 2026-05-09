using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Warehouse.Core.Commands;
using Warehouse.Core.Interfaces;
using Warehouse.Domain.Entities;

namespace Warehouse.Core.Handlers
{
    public class CreateProductCommandHandler
       : IRequestHandler<CreateProductCommand, string>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateProductCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<string> Handle(CreateProductCommand request, CancellationToken ct)
        {
            var dto = request.Model;

            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new Exception("Product name is required");

           
            string idproduct = Guid.NewGuid().ToString();
            string? finalCategoryId = null;
            if (!string.IsNullOrEmpty(dto.selectedCategories))
            {
                try
                {
                    var categoryIds = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(dto.selectedCategories);
                    if (categoryIds.Count() >0 )
                    {
                        finalCategoryId = string.Join(",", categoryIds.ToList());
                    }
                 


                }
                catch { /* Xử lý lỗi parse nếu cần */ }
            }

            // 🧱 Create Product
            var product = new Product
            {
                Id = idproduct,
                Name = dto.Name,
                Description = dto.Description ?? "",
                BarCode = dto.BarCode ?? "",
                CodeSKU = dto.CodeSKU ?? "",
                Unit = dto.UnitCal ?? "",
                // Dùng TryParse để an toàn hơn parse trực tiếp
                SellPrice = decimal.TryParse(dto.SellPrice, out var sp) ? sp : 0,
                ComparativePrice = decimal.TryParse(dto.ComparativePrice, out var cp) ? cp : 0,
                CapitalPrice = decimal.TryParse(dto.CapitalPrice, out var cap) ? cap : 0,
                CategoryId = finalCategoryId, // Gán ID đã xử lý
                BrandId = dto.BrandId,
                CreatedAt = DateTime.Now,
                CreatedBy = "admin",
                UpdatedAt = null
            };

            await _unitOfWork.ProductsRepository.AddAsync(product);
                await _unitOfWork.SaveAsync();
            // 3. Xử lý danh sách ảnh
            if (dto.Images != null && dto.Images.Any())
            {
                var folder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

                for (int i = 0; i < dto.Images.Count; i++)
                {
                    var item = dto.Images[i];
                    var fileName = $"{Guid.NewGuid()}{Path.GetExtension(item.FileName)}";
                    var fullPath = Path.Combine(folder, fileName);

                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        await item.CopyToAsync(stream, ct);
                    }

                    var productImage = new Images
                    {
                        Id = Guid.NewGuid().ToString(),
                        FkId = idproduct,
                        ImageUrl = $"/uploads/{fileName}",
                        // logic isMain: Ảnh đầu tiên (index 0) là ảnh chính
                        IsMain = (i == 0),
                        CreatedAt = DateTime.Now,
                        CreatedBy = "admin",
                        UpdatedAt = null
                    };

                    await _unitOfWork.ImagesRepository.AddAsync(productImage);
                }
            }

            if (dto.Stock != null)
            {
                var inventory = new Inventory
                {
                    Id = Guid.NewGuid().ToString(),
                    ProductId = idproduct,
                    VariantId = null,
                    WarehouseId = "DEFAULT", // hoặc tạo sẵn 1 warehouse

                    StockQuantity = int.TryParse(dto.Stock, out var qty) ? qty : 0,
                    OnOrderQuantity = 0,

                    CreatedAt = DateTime.Now,
                    CreatedBy = "admin"
                };

                await _unitOfWork.InventoryRepository.AddAsync(inventory);
            }
            try
            {
                await _unitOfWork.SaveAsync();
            }
            catch (Exception ex)
            {
                var ok = ex;
                throw;
            }
         
            return product.Id;
        }
      
        public class UpdateProductHandler : IRequestHandler<UpdateProductCommand, bool>
        {
            private readonly IUnitOfWork _unitOfWork;

            public UpdateProductHandler(IUnitOfWork unitOfWork)
            {
                _unitOfWork = unitOfWork;
            }

            public async Task<bool> Handle(UpdateProductCommand request, CancellationToken ct)
            {
                var dto = request.Dto;

                var product = await _unitOfWork.ProductsRepository.GetByIdAsync(request.Id);

                if (product == null)
                    throw new Exception("product not found");
                string? finalCategoryId = null;
                if (!string.IsNullOrEmpty(dto.selectedCategories))
                {
                    try
                    {
                        var categoryIds = Newtonsoft.Json.JsonConvert.DeserializeObject<List    <string>>(dto.selectedCategories);
                        if (categoryIds.Count() > 0)
                        {
                            finalCategoryId = string.Join(",", categoryIds.ToList());
                        }



                    }
                    catch { /* Xử lý lỗi parse nếu cần */ }
                }

                product.Name = dto.Name;
                product.Description = dto.Description;
                product.Unit = dto.UnitCal;
                product.BarCode = dto.BarCode;
                product.CodeSKU = dto.CodeSKU;
                product.ComparativePrice = decimal.Parse(dto.SellPrice);
                product.SellPrice = decimal.Parse( dto.SellPrice);
                product.CapitalPrice = decimal.Parse(dto.CapitalPrice);
                product.UpdatedAt = DateTime.Now;
                product.UpdatedBy = "ok";
                product.CategoryId = finalCategoryId;
                _unitOfWork.ProductsRepository.Update(product);

                // xóa toàn bộ anh
                var imgs = await _unitOfWork.ImagesRepository
                    .Query()
                    .Where(x => x.FkId == product.Id)
                    .ToListAsync(ct);
                _unitOfWork.ImagesRepository.RemoveRange(imgs);
       

                if (dto.Images != null && dto.Images.Any())
                {
                    var folder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                    if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

                    for (int i = 0; i < dto.Images.Count; i++)
                    {
                        var item = dto.Images[i];
                        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(item.FileName)}";
                        var fullPath = Path.Combine(folder, fileName);

                        using (var stream = new FileStream(fullPath, FileMode.Create))
                        {
                            await item.CopyToAsync(stream, ct);
                        }

                        var productImage = new Images
                        {
                            Id = Guid.NewGuid().ToString(),
                            FkId = product.Id,
                            ImageUrl = $"/uploads/{fileName}",
                            // logic isMain: Ảnh đầu tiên (index 0) là ảnh chính
                            IsMain = (i == 0),
                            CreatedAt = DateTime.Now,
                            CreatedBy = "admin",
                            UpdatedAt = null
                        };

                        await _unitOfWork.ImagesRepository.AddAsync(productImage);
                    }
                }

                await _unitOfWork.SaveAsync();

                return true;
            }
        }

        public class DeleteCategoryHandler : IRequestHandler<DeleteCategoryCommand, bool>
        {
            private readonly IUnitOfWork _unitOfWork;

            public DeleteCategoryHandler(IUnitOfWork unitOfWork)
            {
                _unitOfWork = unitOfWork;
            }

            public async Task<bool> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
            {
                //var repo = _unitOfWork.CategoryRepository<Category>();

                var category = await _unitOfWork.CategoryRepository.GetByIdAsync(request.Id);

                if (category == null)
                    throw new Exception("Category not found");

                _unitOfWork.CategoryRepository.Delete(category);
                await _unitOfWork.SaveAsync();
                return true;
            }
        }


        public class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryIdsCommand, bool>
        {
            private readonly IUnitOfWork _unitOfWork;

            public DeleteCategoryCommandHandler(IUnitOfWork unitOfWork)
            {
                _unitOfWork = unitOfWork;
            }

            public async Task<bool> Handle(DeleteCategoryIdsCommand request, CancellationToken cancellationToken)
            {
                var categories = await _unitOfWork.CategoryRepository
                    .Query()
                    .Where(x => request.Ids.Contains(x.Id))
                    .ToListAsync(cancellationToken);

                if (!categories.Any())
                    return false;

                _unitOfWork.CategoryRepository.RemoveRange(categories);

                await _unitOfWork.SaveAsync();

                return true;
            }
        }
    }
}