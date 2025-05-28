using Microsoft.EntityFrameworkCore;
using Pampazon.Models;

namespace Pampazon.Data;

public class PampazonDbContext : DbContext
{
    public PampazonDbContext(DbContextOptions<PampazonDbContext> options) : base(options)
    {
    }

    public DbSet<Client> Clients { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<Receipt> Receipts { get; set; }
    public DbSet<Dispatch> Dispatches { get; set; }
    public DbSet<StockPosition> StockPositions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure relationships and constraints
        modelBuilder.Entity<Order>()
            .HasOne(o => o.Client)
            .WithMany()
            .HasForeignKey(o => o.ClientId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Receipt>()
            .HasOne(r => r.Order)
            .WithOne()
            .HasForeignKey<Receipt>(r => r.OrderId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Dispatch>()
            .HasOne(d => d.Order)
            .WithOne()
            .HasForeignKey<Dispatch>(d => d.OrderId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<StockPosition>()
            .HasOne(sp => sp.Product)
            .WithMany()
            .HasForeignKey(sp => sp.ProductId)
            .OnDelete(DeleteBehavior.Restrict);
    }
} 