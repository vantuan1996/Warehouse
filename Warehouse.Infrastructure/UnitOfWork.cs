using Microsoft.EntityFrameworkCore.Storage;
using Warehouse.Core.Interfaces;
using Warehouse.Domain.Entities;
using Warehouse.Infrastructure.DbContext;
using Warehouse.Infrastructure.Repositories;

namespace Warehouse.Infrastructure
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        private IDbContextTransaction _currentTransaction;
        // Giữ lại các thuộc tính này
        public IGenericRepository<Product> Products { get; }
        public IGenericRepository<ProductVariant> ProductVariants { get; }
        public IGenericRepository<InventorySnapshot> InventorySnapshots { get; }
        public IGenericRepository<Category> CategoryRepository { get; }

        // SỬA TẠI ĐÂY: Thay vì throw lỗi, hãy khởi tạo nó trong Constructor hoặc gán trực tiếp
        public IGenericRepository<Product> ProductsRepository { get; }
        public IGenericRepository<Images> ImagesRepository { get; }

        public IGenericRepository<Inventory> InventoryRepository { get; }


        public IGenericRepository<Customers> CustomersRepository { get; }

        public IGenericRepository<CustomerGroups> CustomerGroupsRepository { get; }

        public IGenericRepository<CustomerAddresses> CustomerAddressesRepository { get; }

        public IGenericRepository<CustomerTaxInfos> CustomerTaxInfosRepository { get; }

        public IGenericRepository<Orders> OrderRepository { get; }

        public IGenericRepository<OrderItem> OrderItemRepository { get; }

        public IGenericRepository<MenuItem> MenuItemRepository { get; }

        public UnitOfWork(AppDbContext context)
        {
            _context = context;

            // Khởi tạo tất cả ở đây
            Products = new GenericRepository<Product>(_context);
            ProductsRepository = Products; // Gán cho cùng một instance để tiết kiệm bộ nhớ

            ProductVariants = new GenericRepository<ProductVariant>(_context);
            InventorySnapshots = new GenericRepository<InventorySnapshot>(_context);
            CategoryRepository = new GenericRepository<Category>(_context);

            // Đừng quên khởi tạo Repository cho Images
            ImagesRepository = new GenericRepository<Images>(_context);
            InventoryRepository = new GenericRepository<Inventory>(_context);
            CustomersRepository = new GenericRepository<Customers>(_context);
            CustomerGroupsRepository = new GenericRepository<CustomerGroups>(_context);
            CustomerAddressesRepository = new GenericRepository<CustomerAddresses>(_context);
            CustomerTaxInfosRepository = new GenericRepository<CustomerTaxInfos>(_context);
            OrderRepository = new GenericRepository<Orders>(_context);
            OrderItemRepository = new GenericRepository<OrderItem>(_context);
            MenuItemRepository = new GenericRepository<MenuItem>(_context);
        }

        public async Task<int> SaveAsync()
        {
            // Lưu ý: Chỉ gọi SaveChangesAsync 1 lần duy nhất trong hàm này
            return await _context.SaveChangesAsync();
        }

        // Nếu Interface yêu cầu cả Save() thì giữ, không thì nên dùng SaveAsync là đủ
        public async Task<int> Save() => await SaveAsync();

        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            try
            {
                await _context.SaveChangesAsync();
                if (_currentTransaction != null)
                {
                    await _currentTransaction.CommitAsync();
                }
            }
            catch
            {
                await RollbackTransactionAsync();
                throw;
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    _currentTransaction.Dispose();
                    _currentTransaction = null;
                }
            }
        }

      
        public Task RollbackTransactionAsync()
        {
            throw new NotImplementedException();
        }
    }
}