using System;
using App.Distrbute.Common.Models;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using ObjectStorage.Sdk.Dtos;
using Socials.Sdk.Dtos;

#nullable disable

namespace App.Distrbute.Common.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DataProtectionKeys",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FriendlyName = table.Column<string>(type: "text", nullable: true),
                    Xml = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataProtectionKeys", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Emails",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Address = table.Column<string>(type: "text", nullable: false),
                    LedgerClientId = table.Column<int>(type: "integer", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Emails", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Outbox",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    AggregateType = table.Column<string>(type: "text", nullable: false),
                    AggregateId = table.Column<string>(type: "text", nullable: false),
                    Type = table.Column<string>(type: "text", nullable: false),
                    Payload = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Outbox", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PostMetrics",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    FollowersCount = table.Column<long>(type: "bigint", nullable: false),
                    ViewCount = table.Column<long>(type: "bigint", nullable: false),
                    LikeCount = table.Column<long>(type: "bigint", nullable: false),
                    CommentCount = table.Column<long>(type: "bigint", nullable: false),
                    SavedCount = table.Column<long>(type: "bigint", nullable: false),
                    ShareCount = table.Column<long>(type: "bigint", nullable: false),
                    QuoteCount = table.Column<long>(type: "bigint", nullable: false),
                    ConversionScore = table.Column<double>(type: "double precision", nullable: false),
                    WeightedEngagementScore = table.Column<double>(type: "double precision", nullable: false),
                    EngagementDepthRatio = table.Column<double>(type: "double precision", nullable: false),
                    AuthenticityScore = table.Column<double>(type: "double precision", nullable: false),
                    EstimatedConversions = table.Column<double>(type: "double precision", nullable: false),
                    EngagementRate = table.Column<double>(type: "double precision", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PostMetrics", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Brands",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Niches = table.Column<string>(type: "text", nullable: false),
                    Bio = table.Column<string>(type: "text", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    RelevanceScore = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    EmailId = table.Column<string>(type: "text", nullable: false),
                    ProfilePicture = table.Column<DocumentFile>(type: "jsonb", nullable: true),
                    Location = table.Column<Location>(type: "jsonb", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Brands", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Brands_Emails_EmailId",
                        column: x => x.EmailId,
                        principalTable: "Emails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Distributors",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    OpenToCollaboration = table.Column<bool>(type: "boolean", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    RelevanceScore = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    EmailId = table.Column<string>(type: "text", nullable: false),
                    ProfilePicture = table.Column<DocumentFile>(type: "jsonb", nullable: true),
                    Location = table.Column<Location>(type: "jsonb", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Distributors", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Distributors_Emails_EmailId",
                        column: x => x.EmailId,
                        principalTable: "Emails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BrandInvites",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: true),
                    Role = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    Token = table.Column<string>(type: "text", nullable: false),
                    Expires = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    BrandId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BrandInvites", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BrandInvites_Brands_BrandId",
                        column: x => x.BrandId,
                        principalTable: "Brands",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BrandMembers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    EmailId = table.Column<string>(type: "text", nullable: false),
                    Role = table.Column<string>(type: "text", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    BrandId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BrandMembers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BrandMembers_Brands_BrandId",
                        column: x => x.BrandId,
                        principalTable: "Brands",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BrandMembers_Emails_EmailId",
                        column: x => x.EmailId,
                        principalTable: "Emails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BrandSocialAccounts",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    BrandId = table.Column<string>(type: "text", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DisplayName = table.Column<string>(type: "text", nullable: false),
                    Handle = table.Column<string>(type: "text", nullable: false),
                    ProfileImageUrl = table.Column<string>(type: "text", nullable: true),
                    ExternalId = table.Column<string>(type: "text", nullable: false),
                    Platform = table.Column<string>(type: "text", nullable: false),
                    ProfileLink = table.Column<string>(type: "text", nullable: false),
                    FollowingCount = table.Column<long>(type: "bigint", nullable: false),
                    FollowersCount = table.Column<long>(type: "bigint", nullable: false),
                    AccessToken = table.Column<string>(type: "text", nullable: false),
                    RefreshToken = table.Column<string>(type: "text", nullable: false),
                    AccessTokenExpiry = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    RefreshTokenExpiry = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    OAuthTokenKind = table.Column<string>(type: "text", nullable: false),
                    LastSynced = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    IsDisabled = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BrandSocialAccounts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BrandSocialAccounts_Brands_BrandId",
                        column: x => x.BrandId,
                        principalTable: "Brands",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DistrbuteTransactions",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    BrandId = table.Column<string>(type: "text", nullable: true),
                    DistributorId = table.Column<string>(type: "text", nullable: true),
                    LedgerActionId = table.Column<int>(type: "integer", nullable: true),
                    Currency = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    IntegrationChannel = table.Column<string>(type: "text", nullable: false),
                    Source = table.Column<string>(type: "jsonb", nullable: false),
                    Destination = table.Column<string>(type: "jsonb", nullable: true),
                    TransactionType = table.Column<string>(type: "text", nullable: false),
                    ClientReference = table.Column<string>(type: "text", nullable: false),
                    Amount = table.Column<double>(type: "double precision", nullable: true),
                    Charges = table.Column<double>(type: "double precision", nullable: true),
                    AmountAfterCharges = table.Column<double>(type: "double precision", nullable: true),
                    AmountDueDistrbute = table.Column<double>(type: "double precision", nullable: true),
                    PaymentProcessor = table.Column<string>(type: "text", nullable: true),
                    PaymentProcessorClientReference = table.Column<string>(type: "text", nullable: true),
                    PaymentProcessorDescription = table.Column<string>(type: "text", nullable: true),
                    TransactionStatus = table.Column<string>(type: "text", nullable: false),
                    TransactionDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    SettledDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Steps = table.Column<string>(type: "jsonb", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DistrbuteTransactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DistrbuteTransactions_Brands_BrandId",
                        column: x => x.BrandId,
                        principalTable: "Brands",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DistrbuteTransactions_Distributors_DistributorId",
                        column: x => x.DistributorId,
                        principalTable: "Distributors",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "DistributorSocialAccounts",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    DistributorId = table.Column<string>(type: "text", nullable: false),
                    StoryPaidViews = table.Column<long>(type: "bigint", nullable: false),
                    ReelPaidViews = table.Column<long>(type: "bigint", nullable: false),
                    ShortPaidViews = table.Column<long>(type: "bigint", nullable: false),
                    PostPaidViews = table.Column<long>(type: "bigint", nullable: false),
                    Niches = table.Column<string>(type: "jsonb", nullable: false),
                    ExcludeFromNiche = table.Column<string>(type: "jsonb", nullable: false),
                    ExcludeFromContent = table.Column<string>(type: "jsonb", nullable: false),
                    AudienceLastUpdated = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DisplayName = table.Column<string>(type: "text", nullable: false),
                    Handle = table.Column<string>(type: "text", nullable: false),
                    ProfileImageUrl = table.Column<string>(type: "text", nullable: true),
                    ExternalId = table.Column<string>(type: "text", nullable: false),
                    Platform = table.Column<string>(type: "text", nullable: false),
                    ProfileLink = table.Column<string>(type: "text", nullable: false),
                    FollowingCount = table.Column<long>(type: "bigint", nullable: false),
                    FollowersCount = table.Column<long>(type: "bigint", nullable: false),
                    AccessToken = table.Column<string>(type: "text", nullable: false),
                    RefreshToken = table.Column<string>(type: "text", nullable: false),
                    AccessTokenExpiry = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    RefreshTokenExpiry = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    OAuthTokenKind = table.Column<string>(type: "text", nullable: false),
                    LastSynced = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    IsDisabled = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DistributorSocialAccounts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DistributorSocialAccounts_Distributors_DistributorId",
                        column: x => x.DistributorId,
                        principalTable: "Distributors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SuspenseWallets",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    DistributorId = table.Column<string>(type: "text", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ProductId = table.Column<int>(type: "integer", nullable: false),
                    AccountId = table.Column<int>(type: "integer", nullable: false),
                    ClientId = table.Column<int>(type: "integer", nullable: false),
                    AccountName = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SuspenseWallets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SuspenseWallets_Distributors_DistributorId",
                        column: x => x.DistributorId,
                        principalTable: "Distributors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Wallets",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    BrandId = table.Column<string>(type: "text", nullable: true),
                    DistributorId = table.Column<string>(type: "text", nullable: true),
                    AccountNumber = table.Column<string>(type: "text", nullable: false),
                    Type = table.Column<string>(type: "text", nullable: false),
                    Provider = table.Column<string>(type: "text", nullable: false),
                    ProviderLogoUrl = table.Column<string>(type: "text", nullable: true),
                    AuthorizationCode = table.Column<string>(type: "text", nullable: true),
                    RecipientCode = table.Column<string>(type: "text", nullable: true),
                    Verified = table.Column<bool>(type: "boolean", nullable: false),
                    Active = table.Column<bool>(type: "boolean", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ProductId = table.Column<int>(type: "integer", nullable: false),
                    AccountId = table.Column<int>(type: "integer", nullable: false),
                    ClientId = table.Column<int>(type: "integer", nullable: false),
                    AccountName = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Wallets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Wallets_Brands_BrandId",
                        column: x => x.BrandId,
                        principalTable: "Brands",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Wallets_Distributors_DistributorId",
                        column: x => x.DistributorId,
                        principalTable: "Distributors",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Campaigns",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Type = table.Column<string>(type: "text", nullable: false),
                    SubType = table.Column<string>(type: "text", nullable: true),
                    Budget = table.Column<double>(type: "double precision", nullable: false),
                    Reach = table.Column<double>(type: "double precision", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    TargetedNiches = table.Column<string>(type: "jsonb", nullable: false),
                    TargetedPlatforms = table.Column<string>(type: "jsonb", nullable: false),
                    Attachment = table.Column<ContentDocumentFile>(type: "jsonb", nullable: true),
                    RulesOfEngagement = table.Column<UGCDocumentFile>(type: "jsonb", nullable: true),
                    ContentType = table.Column<string>(type: "text", nullable: false),
                    FundingTransactionId = table.Column<string>(type: "text", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    BrandId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Campaigns", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Campaigns_Brands_BrandId",
                        column: x => x.BrandId,
                        principalTable: "Brands",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Campaigns_DistrbuteTransactions_FundingTransactionId",
                        column: x => x.FundingTransactionId,
                        principalTable: "DistrbuteTransactions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CampaignInvites",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    DistributorSocialAccountId = table.Column<string>(type: "text", nullable: false),
                    CampaignId = table.Column<string>(type: "text", nullable: false),
                    Ask = table.Column<double>(type: "double precision", nullable: false),
                    Bid = table.Column<double>(type: "double precision", nullable: false),
                    Reach = table.Column<long>(type: "bigint", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    Expiry = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    AcceptedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DeclinedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    EndedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    PostSubmitted = table.Column<bool>(type: "boolean", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    BrandId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CampaignInvites", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CampaignInvites_Brands_BrandId",
                        column: x => x.BrandId,
                        principalTable: "Brands",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CampaignInvites_Campaigns_CampaignId",
                        column: x => x.CampaignId,
                        principalTable: "Campaigns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CampaignInvites_DistributorSocialAccounts_DistributorSocial~",
                        column: x => x.DistributorSocialAccountId,
                        principalTable: "DistributorSocialAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Posts",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    ExternalPostId = table.Column<string>(type: "text", nullable: false),
                    DistributorSocialAccountId = table.Column<string>(type: "text", nullable: true),
                    BrandSocialAccountId = table.Column<string>(type: "text", nullable: true),
                    PostValuationId = table.Column<string>(type: "text", nullable: true),
                    CampaignInviteId = table.Column<string>(type: "text", nullable: true),
                    DistrbuteTransactionId = table.Column<string>(type: "text", nullable: true),
                    PostStatus = table.Column<string>(type: "text", nullable: true),
                    PostApprovalStatus = table.Column<string>(type: "text", nullable: true),
                    PostPayoutStatus = table.Column<string>(type: "text", nullable: true),
                    Embedding = table.Column<Embedding>(type: "jsonb", nullable: true),
                    Link = table.Column<string>(type: "text", nullable: false),
                    ContentType = table.Column<string>(type: "text", nullable: false),
                    PostedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    BrandId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Posts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Posts_BrandSocialAccounts_BrandSocialAccountId",
                        column: x => x.BrandSocialAccountId,
                        principalTable: "BrandSocialAccounts",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Posts_Brands_BrandId",
                        column: x => x.BrandId,
                        principalTable: "Brands",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Posts_CampaignInvites_CampaignInviteId",
                        column: x => x.CampaignInviteId,
                        principalTable: "CampaignInvites",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Posts_DistrbuteTransactions_DistrbuteTransactionId",
                        column: x => x.DistrbuteTransactionId,
                        principalTable: "DistrbuteTransactions",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Posts_DistributorSocialAccounts_DistributorSocialAccountId",
                        column: x => x.DistributorSocialAccountId,
                        principalTable: "DistributorSocialAccounts",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Posts_PostMetrics_PostValuationId",
                        column: x => x.PostValuationId,
                        principalTable: "PostMetrics",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_BrandInvites_BrandId",
                table: "BrandInvites",
                column: "BrandId");

            migrationBuilder.CreateIndex(
                name: "IX_BrandMembers_BrandId",
                table: "BrandMembers",
                column: "BrandId");

            migrationBuilder.CreateIndex(
                name: "IX_BrandMembers_EmailId",
                table: "BrandMembers",
                column: "EmailId");

            migrationBuilder.CreateIndex(
                name: "IX_Brands_EmailId",
                table: "Brands",
                column: "EmailId");

            migrationBuilder.CreateIndex(
                name: "IX_BrandSocialAccounts_BrandId",
                table: "BrandSocialAccounts",
                column: "BrandId");

            migrationBuilder.CreateIndex(
                name: "IX_CampaignInvites_BrandId",
                table: "CampaignInvites",
                column: "BrandId");

            migrationBuilder.CreateIndex(
                name: "IX_CampaignInvites_CampaignId",
                table: "CampaignInvites",
                column: "CampaignId");

            migrationBuilder.CreateIndex(
                name: "IX_CampaignInvites_DistributorSocialAccountId",
                table: "CampaignInvites",
                column: "DistributorSocialAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Campaigns_BrandId",
                table: "Campaigns",
                column: "BrandId");

            migrationBuilder.CreateIndex(
                name: "IX_Campaigns_FundingTransactionId",
                table: "Campaigns",
                column: "FundingTransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_DistrbuteTransactions_BrandId",
                table: "DistrbuteTransactions",
                column: "BrandId");

            migrationBuilder.CreateIndex(
                name: "IX_DistrbuteTransactions_DistributorId",
                table: "DistrbuteTransactions",
                column: "DistributorId");

            migrationBuilder.CreateIndex(
                name: "IX_Distributors_EmailId",
                table: "Distributors",
                column: "EmailId");

            migrationBuilder.CreateIndex(
                name: "IX_DistributorSocialAccounts_DistributorId",
                table: "DistributorSocialAccounts",
                column: "DistributorId");

            migrationBuilder.CreateIndex(
                name: "IX_DistributorSocialAccounts_Platform",
                table: "DistributorSocialAccounts",
                column: "Platform");

            migrationBuilder.CreateIndex(
                name: "IX_DistributorSocialAccounts_PostPaidViews",
                table: "DistributorSocialAccounts",
                column: "PostPaidViews");

            migrationBuilder.CreateIndex(
                name: "IX_DistributorSocialAccounts_ReelPaidViews",
                table: "DistributorSocialAccounts",
                column: "ReelPaidViews");

            migrationBuilder.CreateIndex(
                name: "IX_DistributorSocialAccounts_ShortPaidViews",
                table: "DistributorSocialAccounts",
                column: "ShortPaidViews");

            migrationBuilder.CreateIndex(
                name: "IX_DistributorSocialAccounts_StoryPaidViews",
                table: "DistributorSocialAccounts",
                column: "StoryPaidViews");

            migrationBuilder.CreateIndex(
                name: "IX_Posts_BrandId",
                table: "Posts",
                column: "BrandId");

            migrationBuilder.CreateIndex(
                name: "IX_Posts_BrandSocialAccountId",
                table: "Posts",
                column: "BrandSocialAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Posts_CampaignInviteId",
                table: "Posts",
                column: "CampaignInviteId");

            migrationBuilder.CreateIndex(
                name: "IX_Posts_DistrbuteTransactionId",
                table: "Posts",
                column: "DistrbuteTransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_Posts_DistributorSocialAccountId",
                table: "Posts",
                column: "DistributorSocialAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Posts_PostValuationId",
                table: "Posts",
                column: "PostValuationId");

            migrationBuilder.CreateIndex(
                name: "IX_SuspenseWallets_DistributorId",
                table: "SuspenseWallets",
                column: "DistributorId");

            migrationBuilder.CreateIndex(
                name: "IX_Wallets_BrandId",
                table: "Wallets",
                column: "BrandId");

            migrationBuilder.CreateIndex(
                name: "IX_Wallets_DistributorId",
                table: "Wallets",
                column: "DistributorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BrandInvites");

            migrationBuilder.DropTable(
                name: "BrandMembers");

            migrationBuilder.DropTable(
                name: "DataProtectionKeys");

            migrationBuilder.DropTable(
                name: "Outbox");

            migrationBuilder.DropTable(
                name: "Posts");

            migrationBuilder.DropTable(
                name: "SuspenseWallets");

            migrationBuilder.DropTable(
                name: "Wallets");

            migrationBuilder.DropTable(
                name: "BrandSocialAccounts");

            migrationBuilder.DropTable(
                name: "CampaignInvites");

            migrationBuilder.DropTable(
                name: "PostMetrics");

            migrationBuilder.DropTable(
                name: "Campaigns");

            migrationBuilder.DropTable(
                name: "DistributorSocialAccounts");

            migrationBuilder.DropTable(
                name: "DistrbuteTransactions");

            migrationBuilder.DropTable(
                name: "Brands");

            migrationBuilder.DropTable(
                name: "Distributors");

            migrationBuilder.DropTable(
                name: "Emails");
        }
    }
}
