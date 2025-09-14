using Microsoft.EntityFrameworkCore;
using BrandApi.Models;

namespace BrandApi.Data
{
    public class BrandDbContext : DbContext
    {
        public BrandDbContext(DbContextOptions<BrandDbContext> options) : base(options)
        {
        }

        public DbSet<Brand> Brands { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure relationships
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Brand)
                .WithMany(b => b.Products)
                .HasForeignKey(p => p.BrandId);

            modelBuilder.Entity<Order>()
                .HasOne(o => o.Brand)
                .WithMany()
                .HasForeignKey(o => o.BrandId);

            // Configure indexes
            modelBuilder.Entity<Product>()
                .HasIndex(p => p.Sku)
                .IsUnique();

            modelBuilder.Entity<Brand>()
                .HasIndex(b => b.ContactEmail)
                .IsUnique();
        }
    }
}
