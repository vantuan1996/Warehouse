using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Warehouse.Domain.Entities;

namespace Warehouse.Infrastructure.DbContext
{
    public class AppDbContext : Microsoft.EntityFrameworkCore.DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }

        public DbSet<Images> Images { get; set; }

        public DbSet<Warehouses> Warehouses { get; set; }
        public DbSet<Inventory> Inventory { get; set; }
        public DbSet<Customers> Customers { get; set; }
        public DbSet<CustomerGroups> CustomerGroups { get; set; }
        public DbSet<CustomerAddresses> CustomerAddresses { get; set; }
        public DbSet<CustomerTaxInfos> CustomerTaxInfos { get; set; }
        public DbSet<Orders> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<MenuItem> MenuItems { get; set; }

        //public DbSet<InventorySnapshot> InventorySnapshots { get; set; }

        //public DbSet<InventoryTransaction> InventoryTransactions { get; set; }
        public DbSet<Category> Categories { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Customers>()
        .HasKey(x => x.Id);
            modelBuilder.Entity<CustomerGroups>()
     .HasKey(x => x.Id);

            modelBuilder.Entity<Category>().ToTable("Categories");
            // Ép tên bảng chính xác (phân biệt hoa thường nếu DB của bạn có cấu hình đặc biệt)
            modelBuilder.Entity<Inventory>().ToTable("Inventory");

            // Chỉ định Id là Primary Key vì nó là kiểu String
            modelBuilder.Entity<Inventory>().HasKey(x => x.Id);

            base.OnModelCreating(modelBuilder);
        }
    }
}
