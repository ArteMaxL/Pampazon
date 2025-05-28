using Microsoft.EntityFrameworkCore;
using Pampazon.Models;

namespace Pampazon.Data;

public class PampazonDbContext(DbContextOptions<PampazonDbContext> options) : DbContext(options)
{
    public DbSet<Client> Clients { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<Receipt> Receipts { get; set; }
    public DbSet<ReceiptItem> ReceiptItems { get; set; }
    public DbSet<StockPosition> StockPositions { get; set; }
    public DbSet<Dispatch> Dispatches { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configure Client
        modelBuilder.Entity<Client>()
            .HasKey(c => c.CUIT);

        // Configure Product
        modelBuilder.Entity<Product>()
            .HasKey(p => p.Code);

        // Configure Order
        modelBuilder.Entity<Order>()
            .HasKey(o => o.OrderNumber);

        modelBuilder.Entity<Order>()
            .HasOne(o => o.Client)
            .WithMany()
            .HasForeignKey(o => o.ClientId)
            .OnDelete(DeleteBehavior.Restrict);

        // Configure OrderItem
        modelBuilder.Entity<OrderItem>()
            .HasOne(oi => oi.Order)
            .WithMany(o => o.Items)
            .HasForeignKey(oi => oi.OrderNumber)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<OrderItem>()
            .HasOne(oi => oi.Product)
            .WithMany()
            .HasForeignKey(oi => oi.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        // Configure Receipt
        modelBuilder.Entity<Receipt>()
            .HasKey(r => r.ReceiptNumber);

        modelBuilder.Entity<Receipt>()
            .HasOne(r => r.Client)
            .WithMany()
            .HasForeignKey(r => r.ClientId)
            .OnDelete(DeleteBehavior.Restrict);

        // Configure ReceiptItem
        modelBuilder.Entity<ReceiptItem>()
            .HasOne(ri => ri.Receipt)
            .WithMany(r => r.Items)
            .HasForeignKey(ri => ri.ReceiptNumber)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ReceiptItem>()
            .HasOne(ri => ri.Product)
            .WithMany()
            .HasForeignKey(ri => ri.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        // Configure StockPosition
        modelBuilder.Entity<StockPosition>()
            .HasOne(sp => sp.Product)
            .WithMany()
            .HasForeignKey(sp => sp.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<StockPosition>()
            .HasOne(sp => sp.Client)
            .WithMany()
            .HasForeignKey(sp => sp.ClientId)
            .OnDelete(DeleteBehavior.Restrict);

        // Configure Dispatch
        modelBuilder.Entity<Dispatch>()
            .HasKey(d => d.DispatchNumber);

        modelBuilder.Entity<Dispatch>()
            .HasOne(d => d.Order)
            .WithMany()
            .HasForeignKey(d => d.OrderNumber)
            .OnDelete(DeleteBehavior.Restrict);

        base.OnModelCreating(modelBuilder);
    }
}
