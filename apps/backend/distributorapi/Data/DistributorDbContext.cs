using Microsoft.EntityFrameworkCore;
using DistributorApi.Models;

namespace DistributorApi.Data
{
    public class DistributorDbContext : DbContext
    {
        public DistributorDbContext(DbContextOptions<DistributorDbContext> options) : base(options)
        {
        }

        public DbSet<Distributor> Distributors { get; set; }
        public DbSet<Order> Orders { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure relationships
            modelBuilder.Entity<Order>()
                .HasOne(o => o.Distributor)
                .WithMany(d => d.Orders)
                .HasForeignKey(o => o.DistributorId);

            // Configure indexes
            modelBuilder.Entity<Distributor>()
                .HasIndex(d => d.ContactEmail)
                .IsUnique();
        }
    }
}
