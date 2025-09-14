using System;
using App.Distrbute.Common.Models;
using Microsoft.EntityFrameworkCore.Migrations;
using ObjectStorage.Sdk.Dtos;

#nullable disable

namespace App.Distrbute.Common.Migrations
{
    /// <inheritdoc />
    public partial class add_more_models : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
                name: "SuspenseWallets",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    DistributorId = table.Column<string>(type: "text", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    EmailId = table.Column<string>(type: "text", nullable: false),
                    ProductId = table.Column<int>(type: "integer", nullable: false),
                    AccountId = table.Column<int>(type: "integer", nullable: false),
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
                    table.ForeignKey(
                        name: "FK_SuspenseWallets_Emails_EmailId",
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
                name: "DistrbuteTransactions",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    SavingsClientId = table.Column<int>(type: "integer", nullable: true),
                    SavingsProductId = table.Column<int>(type: "integer", nullable: true),
                    SavingsAccountId = table.Column<int>(type: "integer", nullable: true),
                    BrandId = table.Column<string>(type: "text", nullable: true),
                    DistributorId = table.Column<string>(type: "text", nullable: true),
                    LedgerActionId = table.Column<int>(type: "integer", nullable: true),
                    Currency = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    IntegrationChannel = table.Column<string>(type: "text", nullable: false),
                    Source = table.Column<Depository>(type: "jsonb", nullable: true),
                    Destination = table.Column<Depository>(type: "jsonb", nullable: true),
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
                    EmailId = table.Column<string>(type: "text", nullable: false),
                    ProductId = table.Column<int>(type: "integer", nullable: false),
                    AccountId = table.Column<int>(type: "integer", nullable: false),
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
                    table.ForeignKey(
                        name: "FK_Wallets_Emails_EmailId",
                        column: x => x.EmailId,
                        principalTable: "Emails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Campaign",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Type = table.Column<string>(type: "text", nullable: false),
                    SubType = table.Column<string>(type: "text", nullable: true),
                    TargetedNiches = table.Column<string>(type: "jsonb", nullable: false),
                    TargetedPlatforms = table.Column<string>(type: "jsonb", nullable: false),
                    Attachment = table.Column<string>(type: "jsonb", nullable: true),
                    RulesOfEngagement = table.Column<UGCDocumentFile>(type: "jsonb", nullable: true),
                    Budget = table.Column<double>(type: "double precision", nullable: false),
                    Reach = table.Column<double>(type: "double precision", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    FundingTransactionId = table.Column<string>(type: "text", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    BrandId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Campaign", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Campaign_Brands_BrandId",
                        column: x => x.BrandId,
                        principalTable: "Brands",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Campaign_DistrbuteTransactions_FundingTransactionId",
                        column: x => x.FundingTransactionId,
                        principalTable: "DistrbuteTransactions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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
                name: "IX_Campaign_BrandId",
                table: "Campaign",
                column: "BrandId");

            migrationBuilder.CreateIndex(
                name: "IX_Campaign_FundingTransactionId",
                table: "Campaign",
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
                name: "IX_SuspenseWallets_DistributorId",
                table: "SuspenseWallets",
                column: "DistributorId");

            migrationBuilder.CreateIndex(
                name: "IX_SuspenseWallets_EmailId",
                table: "SuspenseWallets",
                column: "EmailId");

            migrationBuilder.CreateIndex(
                name: "IX_Wallets_BrandId",
                table: "Wallets",
                column: "BrandId");

            migrationBuilder.CreateIndex(
                name: "IX_Wallets_DistributorId",
                table: "Wallets",
                column: "DistributorId");

            migrationBuilder.CreateIndex(
                name: "IX_Wallets_EmailId",
                table: "Wallets",
                column: "EmailId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BrandInvites");

            migrationBuilder.DropTable(
                name: "BrandMembers");

            migrationBuilder.DropTable(
                name: "Campaign");

            migrationBuilder.DropTable(
                name: "SuspenseWallets");

            migrationBuilder.DropTable(
                name: "Wallets");

            migrationBuilder.DropTable(
                name: "DistrbuteTransactions");

            migrationBuilder.DropTable(
                name: "Brands");
        }
    }
}
