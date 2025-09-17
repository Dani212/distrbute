using System.Linq.Expressions;
using App.Distrbute.Common.Enums;
using App.Distrbute.Common.Models;
using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Newtonsoft.Json;
using Persistence.Sdk.Models;
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
    public DbSet<Email> Emails { get; set; }
    public DbSet<Brand> Brands { get; set; }
    public DbSet<BrandMember> BrandMembers { get; set; }
    public DbSet<BrandInvite> BrandInvites { get; set; }
    public DbSet<Distributor> Distributors { get; set; }
    // public DbSet<SocialAccount> SocialAccounts { get; set; }
    // public DbSet<PostValuation> PostValuations { get; set; }
    // public DbSet<Campaign> Campaigns { get; set; }
    // public DbSet<Post> Ads { get; set; }
    public DbSet<Outbox> Outboxes { get; set; }
    public DbSet<Wallet> Wallets { get; set; }
    public DbSet<SuspenseWallet> SuspenseWallets { get; set; }
    public DbSet<DistrbuteTransaction> DistrbuteTransactions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // // indexes
        // modelBuilder.Entity<SocialAccount>()
        //     .HasIndex(s => s.PaidViews);
        // modelBuilder.Entity<SocialAccount>()
        //     .HasIndex(s => s.Platform);

        // constraints
        // compound unique index
        // modelBuilder.Entity<Wallet>()
        //     // same wallet can belong to a distributor or a brand, but a distributor or brand cannot have same wallet twice
        //     .HasIndex(w => new { w.Email, w.Brand.Id, w.Distributor, w.AccountNumber, w.Provider, w.Type })
        //     .IsUnique();

        // // POST VALUATION
        // modelBuilder.Entity<PostValuation>()
        //     // same wallet can belong to a distributor or a brand, but a distributor or brand cannot have same wallet twice
        //     .HasIndex(w => new { w.ExternalPostId })
        //     .IsUnique();
        //
        // modelBuilder.Entity<PostValuation>()
        //     .Property(e => e.ContentType)
        //     .HasConversion<string>();

        // transaction
        modelBuilder.Entity<DistrbuteTransaction>()
            .Property(e => e.PaymentProcessor)
            .HasConversion<string>();

        modelBuilder.Entity<DistrbuteTransaction>()
            .Property(e => e.TransactionStatus)
            .HasConversion<string>();

        modelBuilder.Entity<DistrbuteTransaction>()
            .Property(e => e.TransactionType)
            .HasConversion<string>();

        modelBuilder.Entity<DistrbuteTransaction>()
            .Property(e => e.Steps)
            .HasConversion(GetValueConverter<DistrbuteTransaction, List<Step>>(d => d.Steps))
            .Metadata.SetValueComparer(GetValueComparer<DistrbuteTransaction, List<Step>>(d => d.Steps));

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
            .Property(e => e.TargetedNiches)
            .HasConversion(GetValueConverter<Campaign, List<DistributorNiche>>(d => d.TargetedNiches))
            .Metadata.SetValueComparer(GetValueComparer<Campaign, List<DistributorNiche>>(d => d.TargetedNiches));

        modelBuilder.Entity<Campaign>()
            .Property(e => e.TargetedPlatforms)
            .HasConversion(GetValueConverter<Campaign, List<PlatformSplit>>(d => d.TargetedPlatforms))
            .Metadata.SetValueComparer(GetValueComparer<Campaign, List<PlatformSplit>>(d => d.TargetedPlatforms));

        modelBuilder.Entity<Campaign>()
            .Property(e => e.Attachment)
            .HasConversion(GetValueConverter<Campaign, ContentDocumentFile>(d => d.Attachment))
            .Metadata.SetValueComparer(GetValueComparer<Campaign, ContentDocumentFile>(d => d.Attachment));

        // // campaign invite
        // modelBuilder.Entity<CampaignInvite>()
        //     .Property(e => e.CampaignType)
        //     .HasConversion<string>();
        //
        // modelBuilder.Entity<CampaignInvite>()
        //     .Property(e => e.Platform)
        //     .HasConversion<string>();
        //
        // modelBuilder.Entity<CampaignInvite>()
        //     .Property(e => e.Status)
        //     .HasConversion<string>();
        //
        // modelBuilder.Entity<CampaignInvite>()
        //     .Property(e => e.Attachment)
        //     .HasConversion(GetValueConverter<CampaignInvite, ContentDocumentFile>(d => d.Attachment))
        //     .Metadata.SetValueComparer(GetValueComparer<CampaignInvite, ContentDocumentFile>(d => d.Attachment));
        //
        // // post
        // modelBuilder.Entity<Post>()
        //     .Property(e => e.CampaignType)
        //     .HasConversion<string>();
        //
        // modelBuilder.Entity<Post>()
        //     .Property(e => e.Platform)
        //     .HasConversion<string>();
        //
        // modelBuilder.Entity<Post>()
        //     .Property(e => e.AdStatus)
        //     .HasConversion<string>();
        //
        // modelBuilder.Entity<Post>()
        //     .Property(e => e.AdApprovalStatus)
        //     .HasConversion<string>();
        //
        // modelBuilder.Entity<Post>()
        //     .Property(e => e.AdPayoutStatus)
        //     .HasConversion<string>();
        //
        // modelBuilder.Entity<Post>()
        //     .Property(e => e.Attachment)
        //     .HasConversion(GetValueConverter<Post, ContentDocumentFile>(d => d.Attachment))
        //     .Metadata.SetValueComparer(GetValueComparer<Post, ContentDocumentFile>(d => d.Attachment));

        // brand
        modelBuilder.Entity<Brand>()
            .Property(e => e.Niches)
            .HasConversion(GetValueConverter<Brand, List<BrandNiche>>(d => d.Niches))
            .Metadata.SetValueComparer(GetValueComparer<Brand, List<BrandNiche>>(d => d.Niches));

        // Brand member
        modelBuilder.Entity<BrandMember>()
            .Property(e => e.Role)
            .HasConversion<string>();

        // Brand invite
        modelBuilder.Entity<BrandInvite>()
            .Property(e => e.Role)
            .HasConversion<string>();

        modelBuilder.Entity<BrandInvite>()
            .Property(e => e.Status)
            .HasConversion<string>();

        // // social account
        // modelBuilder.Entity<SocialAccount>()
        //     .Property(e => e.Platform)
        //     .HasConversion<string>();
        //
        // modelBuilder.Entity<SocialAccount>()
        //     .Property(e => e.AudienceAgeGroups)
        //     .HasConversion(GetValueConverter<SocialAccount, List<AgeGroup>>(d => d.AudienceAgeGroups))
        //     .Metadata.SetValueComparer(GetValueComparer<SocialAccount, List<AgeGroup>>(d => d.AudienceAgeGroups));
        //
        // modelBuilder.Entity<SocialAccount>()
        //     .Property(e => e.AudienceGenders)
        //     .HasConversion(GetValueConverter<SocialAccount, List<AudienceGender>>(d => d.AudienceGenders))
        //     .Metadata.SetValueComparer(GetValueComparer<SocialAccount, List<AudienceGender>>(d => d.AudienceGenders));
        //
        // modelBuilder.Entity<SocialAccount>()
        //     .Property(e => e.Audience)
        //     .HasConversion(GetValueConverter<SocialAccount, List<AudiencePersona>>(d => d.Audience))
        //     .Metadata.SetValueComparer(GetValueComparer<SocialAccount, List<AudiencePersona>>(d => d.Audience));
        //
        // modelBuilder.Entity<SocialAccount>()
        //     .Property(e => e.AudienceLocations)
        //     .HasConversion(GetValueConverter<SocialAccount, List<AudienceLocation>>(d => d.AudienceLocations))
        //     .Metadata.SetValueComparer(
        //         GetValueComparer<SocialAccount, List<AudienceLocation>>(d => d.AudienceLocations));
        //
        // modelBuilder.Entity<SocialAccount>()
        //     .Property(e => e.ExcludeFromNiche)
        //     .HasConversion(GetValueConverter<SocialAccount, List<Niche>>(d => d.ExcludeFromNiche))
        //     .Metadata.SetValueComparer(GetValueComparer<SocialAccount, List<Niche>>(d => d.ExcludeFromNiche));
        //
        // modelBuilder.Entity<SocialAccount>()
        //     .Property(e => e.ExcludeFromContent)
        //     .HasConversion(GetValueConverter<SocialAccount, List<ContentType>>(d => d.ExcludeFromContent))
        //     .Metadata.SetValueComparer(
        //         GetValueComparer<SocialAccount, List<ContentType>>(d => d.ExcludeFromContent));

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