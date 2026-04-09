using Microsoft.EntityFrameworkCore;
using SportsStore.Core.Domain.Entities;

namespace SportsStore.OrderApi.Data;

public class OrderDbContext : DbContext
{
    public OrderDbContext(DbContextOptions<OrderDbContext> options) : base(options) { }

    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<InventoryRecord> InventoryRecords { get; set; }
    public DbSet<PaymentRecord> PaymentRecords { get; set; }
    public DbSet<ShipmentRecord> ShipmentRecords { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.OrderID);
            entity.Property(e => e.OrderID).ValueGeneratedOnAdd();
            entity.HasMany(e => e.Items)
                .WithOne(i => i.Order)
                .HasForeignKey(i => i.OrderID);
            entity.HasOne(e => e.InventoryRecord)
                .WithOne()
                .HasForeignKey<InventoryRecord>(i => i.OrderId);
            entity.HasOne(e => e.PaymentRecord)
                .WithOne()
                .HasForeignKey<PaymentRecord>(p => p.OrderId);
            entity.HasOne(e => e.ShipmentRecord)
                .WithOne()
                .HasForeignKey<ShipmentRecord>(s => s.OrderId);
        });

        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.HasKey(e => e.OrderItemID);
            entity.Property(e => e.OrderItemID).ValueGeneratedOnAdd();
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.ProductID);
            entity.Property(e => e.ProductID).ValueGeneratedOnAdd();
        });

        modelBuilder.Entity<InventoryRecord>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
        });

        modelBuilder.Entity<PaymentRecord>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
        });

        modelBuilder.Entity<ShipmentRecord>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
        });

        // Seed products
        modelBuilder.Entity<Product>().HasData(
            new Product
            {
                ProductID = 1,
                Name = "Kayak",
                Description = "A boat for one person",
                Category = "Watersports",
                Price = 275,
                StockQuantity = 100
            },
            new Product
            {
                ProductID = 2,
                Name = "Lifejacket",
                Description = "Protective and fashionable",
                Category = "Watersports",
                Price = 48.95m,
                StockQuantity = 200
            },
            new Product
            {
                ProductID = 3,
                Name = "Soccer Ball",
                Description = "FIFA-approved size and weight",
                Category = "Soccer",
                Price = 19.50m,
                StockQuantity = 150
            },
            new Product
            {
                ProductID = 4,
                Name = "Corner Flags",
                Description = "Give your playing field a professional touch",
                Category = "Soccer",
                Price = 34.95m,
                StockQuantity = 100
            },
            new Product
            {
                ProductID = 5,
                Name = "Stadium",
                Description = "Flat-packed 35,000-seat stadium",
                Category = "Soccer",
                Price = 79500,
                StockQuantity = 10
            },
            new Product
            {
                ProductID = 6,
                Name = "Thinking Cap",
                Description = "Improve brain efficiency by 75%",
                Category = "Chess",
                Price = 16,
                StockQuantity = 50
            },
            new Product
            {
                ProductID = 7,
                Name = "Unsteady Chair",
                Description = "Secretly give your opponent a disadvantage",
                Category = "Chess",
                Price = 29.95m,
                StockQuantity = 75
            },
            new Product
            {
                ProductID = 8,
                Name = "Human Chess Board",
                Description = "A fun game for the family",
                Category = "Chess",
                Price = 75,
                StockQuantity = 30
            },
            new Product
            {
                ProductID = 9,
                Name = "Bling-Bling King",
                Description = "Gold-plated, diamond-studded King",
                Category = "Chess",
                Price = 1200,
                StockQuantity = 15
            }
        );
    }
}
