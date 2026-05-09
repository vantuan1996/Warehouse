using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Warehouse.Domain.Entities;

namespace Warehouse.Core.Interfaces
{
    public interface IUnitOfWork
    {
        IGenericRepository<Product> ProductsRepository { get; }
        IGenericRepository<Images> ImagesRepository { get; }

        IGenericRepository<ProductVariant> ProductVariants { get; }

        IGenericRepository<InventorySnapshot> InventorySnapshots { get; }
        IGenericRepository<Category> CategoryRepository { get; }
        IGenericRepository<Inventory> InventoryRepository { get; }
        IGenericRepository<Customers> CustomersRepository { get; }
        IGenericRepository<CustomerGroups> CustomerGroupsRepository { get; }
        IGenericRepository<CustomerAddresses> CustomerAddressesRepository { get; }
        IGenericRepository<CustomerTaxInfos> CustomerTaxInfosRepository { get; }
        IGenericRepository<Orders> OrderRepository { get; }
        IGenericRepository<OrderItem> OrderItemRepository { get; }
        IGenericRepository<MenuItem> MenuItemRepository { get; }
        Task<IDbContextTransaction> BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
        Task<int> Save();
        Task<int> SaveAsync();
    }
}
