using App.Distrbute.Common.Models;
using App.Distrbute.Common.Models.Distributor;
using Microsoft.EntityFrameworkCore;
using Persistence.Sdk.Models;

namespace App.Distrbute.Common;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        AppContext.SetSwitch("Npgsql.DisableDateTimeInfinityConversions", true);
    }

    // always append s else upserts will break
    public DbSet<Email> Emails { get; set; }
    public DbSet<Distributor> Distributors { get; set; }
    public DbSet<Outbox> Outboxes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
    }
}