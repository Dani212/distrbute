using System.Linq.Expressions;
using App.Distrbute.Common.Models;
using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Newtonsoft.Json;
using Persistence.Sdk.Models;
using Socials.Sdk.Enums;
using Utility.Sdk.Extensions;

namespace App.Distrbute.Common;

public class ApplicationDbContext : DbContext, IDataProtectionKeyContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        AppContext.SetSwitch("Npgsql.DisableDateTimeInfinityConversions", true);
    }
    
    // Data Protection
    public DbSet<DataProtectionKey> DataProtectionKeys { get; set; } = null!;

    // always append s else upserts will break
    public DbSet<Brand> Brands { get; set; }
    public DbSet<BrandDistributorNicheCorrelation> BrandDistributorNicheCorrelations { get; set; }
    public DbSet<BrandInvite> BrandInvites { get; set; }
    public DbSet<BrandMember> BrandMembers { get; set; }
    public DbSet<BrandNiche> BrandNiches { get; set; }
    public DbSet<BrandSocialAccount> BrandSocialAccounts { get; set; }
    public DbSet<Campaign> Campaigns { get; set; }
    public DbSet<CampaignInvite> CampaignInvites { get; set; }
    public DbSet<DistrbuteTransaction> DistrbuteTransactions { get; set; }
    public DbSet<Distributor> Distributors { get; set; }
    public DbSet<DistributorNiche> DistributorNiches { get; set; }
    public DbSet<DistributorSocialAccount> DistributorSocialAccounts { get; set; }
    public DbSet<Email> Emails { get; set; }
    public DbSet<Post> Posts { get; set; }
    public DbSet<PostMetric> PostMetrics { get; set; }
    public DbSet<SuspenseWallet> SuspenseWallets { get; set; }
    public DbSet<Wallet> Wallets { get; set; }
    
    // From persistence sdk
    public DbSet<Outbox> Outboxes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Brand member
        modelBuilder.Entity<BrandMember>()
            .Property(e => e.Role)
            .HasConversion<string>();
        
        // BrandNiche
        modelBuilder.Entity<BrandNiche>()
            .HasIndex(e => e.Name)
            .IsUnique();

        // Brand invite
        modelBuilder.Entity<BrandInvite>()
            .Property(e => e.Role)
            .HasConversion<string>();

        modelBuilder.Entity<BrandInvite>()
            .Property(e => e.Status)
            .HasConversion<string>();
        
        // Brand Social Account
        modelBuilder.Entity<BrandSocialAccount>()
            .Property(e => e.Platform)
            .HasConversion<string>();
        
        modelBuilder.Entity<BrandSocialAccount>()
            .Property(e => e.OAuthTokenKind)
            .HasConversion<string>();
        
        // campaign
        modelBuilder.Entity<Campaign>()
            .Property(e => e.Type)
            .HasConversion<string>();

        modelBuilder.Entity<Campaign>()
            .Property(e => e.SubType)
            .HasConversion<string>();

        modelBuilder.Entity<Campaign>()
            .Property(e => e.Status)
            .HasConversion<string>();

        modelBuilder.Entity<Campaign>()
            .Property(e => e.TargetedPlatforms)
            .HasConversion(GetValueConverter<Campaign, List<PlatformSplit>>(d => d.TargetedPlatforms))
            .Metadata.SetValueComparer(GetValueComparer<Campaign, List<PlatformSplit>>(d => d.TargetedPlatforms));

        modelBuilder.Entity<Campaign>()
            .Property(e => e.ContentType)
            .HasConversion<string>();

        // campaign invite
        modelBuilder.Entity<CampaignInvite>()
            .Property(e => e.Status)
            .HasConversion<string>();
        
        // DistrbuteTransaction
        modelBuilder.Entity<DistrbuteTransaction>()
            .Property(e => e.Source)
            .HasConversion(GetValueConverter<DistrbuteTransaction, Depository>(d => d.Source))
            .Metadata.SetValueComparer(GetValueComparer<DistrbuteTransaction, Depository>(d => d.Source));
        
        modelBuilder.Entity<DistrbuteTransaction>()
            .Property(e => e.Destination)
            .HasConversion(GetValueConverter<DistrbuteTransaction, Depository>(d => d.Destination))
            .Metadata.SetValueComparer(GetValueComparer<DistrbuteTransaction, Depository>(d => d.Destination));
        
        modelBuilder.Entity<DistrbuteTransaction>()
            .Property(e => e.TransactionType)
            .HasConversion<string>();
        
        modelBuilder.Entity<DistrbuteTransaction>()
            .Property(e => e.PaymentProcessor)
            .HasConversion<string>();

        modelBuilder.Entity<DistrbuteTransaction>()
            .Property(e => e.TransactionStatus)
            .HasConversion<string>();

        modelBuilder.Entity<DistrbuteTransaction>()
            .Property(e => e.Steps)
            .HasConversion(GetValueConverter<DistrbuteTransaction, List<Step>>(d => d.Steps))
            .Metadata.SetValueComparer(GetValueComparer<DistrbuteTransaction, List<Step>>(d => d.Steps));
        
        // DistributorNiche
        modelBuilder.Entity<DistributorNiche>()
            .HasIndex(e => e.Name)
            .IsUnique();
        
        // social account
            // Indexes
        modelBuilder.Entity<DistributorSocialAccount>()
            .HasIndex(s => s.Platform);
        
        modelBuilder.Entity<DistributorSocialAccount>()
            .HasIndex(s => s.StoryPaidViews);
        
        modelBuilder.Entity<DistributorSocialAccount>()
            .HasIndex(s => s.ReelPaidViews);
        
        modelBuilder.Entity<DistributorSocialAccount>()
            .HasIndex(s => s.ShortPaidViews);
        
        modelBuilder.Entity<DistributorSocialAccount>()
            .HasIndex(s => s.PostPaidViews);
        
            // Other
        modelBuilder.Entity<DistributorSocialAccount>()
            .Property(e => e.Platform)
            .HasConversion<string>();
        
        modelBuilder.Entity<DistributorSocialAccount>()
            .Property(e => e.OAuthTokenKind)
            .HasConversion<string>();
        
        modelBuilder.Entity<DistributorSocialAccount>()
            .Property(e => e.ExcludeFromContent)
            .HasConversion(GetValueConverter<DistributorSocialAccount, List<ContentType>>(d => d.ExcludeFromContent))
            .Metadata.SetValueComparer(
                GetValueComparer<DistributorSocialAccount, List<ContentType>>(d => d.ExcludeFromContent));
        
        // post
        modelBuilder.Entity<Post>()
            .Property(e => e.PostStatus)
            .HasConversion<string>();
        
        modelBuilder.Entity<Post>()
            .Property(e => e.PostApprovalStatus)
            .HasConversion<string>();
        
        modelBuilder.Entity<Post>()
            .Property(e => e.PostPayoutStatus)
            .HasConversion<string>();
        
        modelBuilder.Entity<Post>()
            .Property(e => e.ContentType)
            .HasConversion<string>();
        
        // wallet
        modelBuilder.Entity<Wallet>()
            .Property(e => e.Type)
            .HasConversion<string>();

        modelBuilder.Entity<Wallet>()
            .Property(e => e.Provider)
            .HasConversion<string>();
    }

    private ValueConverter<TProperty, string> GetValueConverter<TEntity, TProperty>(
        Expression<Func<TEntity, TProperty>> propertyExpression)
        where TEntity : notnull
        where TProperty : notnull
    {
        return new ValueConverter<TProperty, string>(
            v => v.Serialize(null),
            v => JsonConvert.DeserializeObject<TProperty>(v)!
        );
    }

    private ValueComparer<TProperty> GetValueComparer<TEntity, TProperty>(
        Expression<Func<TEntity, TProperty>> propertyExpression)
        where TEntity : notnull
        where TProperty : notnull
    {
        return new ValueComparer<TProperty>(
            (c1, c2) => c1.Serialize(null) == c2.Serialize(null),
            c => c.Serialize(null).GetHashCode(),
            c => JsonConvert.DeserializeObject<TProperty>(c.Serialize(null))!
        );
    }
}