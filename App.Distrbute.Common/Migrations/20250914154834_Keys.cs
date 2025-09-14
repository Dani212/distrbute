using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace App.Distrbute.Common.Migrations
{
    /// <inheritdoc />
    public partial class Keys : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BrandMembers_Brands_Id",
                table: "BrandMembers");

            migrationBuilder.DropForeignKey(
                name: "FK_BrandMembers_Emails_Id",
                table: "BrandMembers");

            migrationBuilder.DropForeignKey(
                name: "FK_Brands_Emails_Id",
                table: "Brands");

            migrationBuilder.DropForeignKey(
                name: "FK_Campaign_Brands_Id",
                table: "Campaign");

            migrationBuilder.DropForeignKey(
                name: "FK_DistrbuteTransactions_Brands_Id",
                table: "DistrbuteTransactions");

            migrationBuilder.DropForeignKey(
                name: "FK_DistrbuteTransactions_Distributors_Id",
                table: "DistrbuteTransactions");

            migrationBuilder.DropForeignKey(
                name: "FK_Distributors_Emails_Id",
                table: "Distributors");

            migrationBuilder.DropForeignKey(
                name: "FK_SuspenseWallets_Emails_Id",
                table: "SuspenseWallets");

            migrationBuilder.DropForeignKey(
                name: "FK_Wallets_Brands_Id",
                table: "Wallets");

            migrationBuilder.DropForeignKey(
                name: "FK_Wallets_Distributors_Id",
                table: "Wallets");

            migrationBuilder.DropForeignKey(
                name: "FK_Wallets_Emails_Id",
                table: "Wallets");

            migrationBuilder.AddColumn<string>(
                name: "BrandId",
                table: "Wallets",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DistributorId",
                table: "Wallets",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EmailId",
                table: "Wallets",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "EmailId",
                table: "SuspenseWallets",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "EmailId",
                table: "Distributors",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "BrandId",
                table: "DistrbuteTransactions",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DistributorId",
                table: "DistrbuteTransactions",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BrandId",
                table: "Campaign",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "EmailId",
                table: "Brands",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "BrandId",
                table: "BrandMembers",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "EmailId",
                table: "BrandMembers",
                type: "text",
                nullable: false,
                defaultValue: "");

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

            migrationBuilder.CreateIndex(
                name: "IX_SuspenseWallets_EmailId",
                table: "SuspenseWallets",
                column: "EmailId");

            migrationBuilder.CreateIndex(
                name: "IX_Distributors_EmailId",
                table: "Distributors",
                column: "EmailId");

            migrationBuilder.CreateIndex(
                name: "IX_DistrbuteTransactions_BrandId",
                table: "DistrbuteTransactions",
                column: "BrandId");

            migrationBuilder.CreateIndex(
                name: "IX_DistrbuteTransactions_DistributorId",
                table: "DistrbuteTransactions",
                column: "DistributorId");

            migrationBuilder.CreateIndex(
                name: "IX_Campaign_BrandId",
                table: "Campaign",
                column: "BrandId");

            migrationBuilder.CreateIndex(
                name: "IX_Brands_EmailId",
                table: "Brands",
                column: "EmailId");

            migrationBuilder.CreateIndex(
                name: "IX_BrandMembers_BrandId",
                table: "BrandMembers",
                column: "BrandId");

            migrationBuilder.CreateIndex(
                name: "IX_BrandMembers_EmailId",
                table: "BrandMembers",
                column: "EmailId");

            migrationBuilder.AddForeignKey(
                name: "FK_BrandMembers_Brands_BrandId",
                table: "BrandMembers",
                column: "BrandId",
                principalTable: "Brands",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BrandMembers_Emails_EmailId",
                table: "BrandMembers",
                column: "EmailId",
                principalTable: "Emails",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Brands_Emails_EmailId",
                table: "Brands",
                column: "EmailId",
                principalTable: "Emails",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Campaign_Brands_BrandId",
                table: "Campaign",
                column: "BrandId",
                principalTable: "Brands",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DistrbuteTransactions_Brands_BrandId",
                table: "DistrbuteTransactions",
                column: "BrandId",
                principalTable: "Brands",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_DistrbuteTransactions_Distributors_DistributorId",
                table: "DistrbuteTransactions",
                column: "DistributorId",
                principalTable: "Distributors",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Distributors_Emails_EmailId",
                table: "Distributors",
                column: "EmailId",
                principalTable: "Emails",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SuspenseWallets_Emails_EmailId",
                table: "SuspenseWallets",
                column: "EmailId",
                principalTable: "Emails",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Wallets_Brands_BrandId",
                table: "Wallets",
                column: "BrandId",
                principalTable: "Brands",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Wallets_Distributors_DistributorId",
                table: "Wallets",
                column: "DistributorId",
                principalTable: "Distributors",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Wallets_Emails_EmailId",
                table: "Wallets",
                column: "EmailId",
                principalTable: "Emails",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BrandMembers_Brands_BrandId",
                table: "BrandMembers");

            migrationBuilder.DropForeignKey(
                name: "FK_BrandMembers_Emails_EmailId",
                table: "BrandMembers");

            migrationBuilder.DropForeignKey(
                name: "FK_Brands_Emails_EmailId",
                table: "Brands");

            migrationBuilder.DropForeignKey(
                name: "FK_Campaign_Brands_BrandId",
                table: "Campaign");

            migrationBuilder.DropForeignKey(
                name: "FK_DistrbuteTransactions_Brands_BrandId",
                table: "DistrbuteTransactions");

            migrationBuilder.DropForeignKey(
                name: "FK_DistrbuteTransactions_Distributors_DistributorId",
                table: "DistrbuteTransactions");

            migrationBuilder.DropForeignKey(
                name: "FK_Distributors_Emails_EmailId",
                table: "Distributors");

            migrationBuilder.DropForeignKey(
                name: "FK_SuspenseWallets_Emails_EmailId",
                table: "SuspenseWallets");

            migrationBuilder.DropForeignKey(
                name: "FK_Wallets_Brands_BrandId",
                table: "Wallets");

            migrationBuilder.DropForeignKey(
                name: "FK_Wallets_Distributors_DistributorId",
                table: "Wallets");

            migrationBuilder.DropForeignKey(
                name: "FK_Wallets_Emails_EmailId",
                table: "Wallets");

            migrationBuilder.DropIndex(
                name: "IX_Wallets_BrandId",
                table: "Wallets");

            migrationBuilder.DropIndex(
                name: "IX_Wallets_DistributorId",
                table: "Wallets");

            migrationBuilder.DropIndex(
                name: "IX_Wallets_EmailId",
                table: "Wallets");

            migrationBuilder.DropIndex(
                name: "IX_SuspenseWallets_EmailId",
                table: "SuspenseWallets");

            migrationBuilder.DropIndex(
                name: "IX_Distributors_EmailId",
                table: "Distributors");

            migrationBuilder.DropIndex(
                name: "IX_DistrbuteTransactions_BrandId",
                table: "DistrbuteTransactions");

            migrationBuilder.DropIndex(
                name: "IX_DistrbuteTransactions_DistributorId",
                table: "DistrbuteTransactions");

            migrationBuilder.DropIndex(
                name: "IX_Campaign_BrandId",
                table: "Campaign");

            migrationBuilder.DropIndex(
                name: "IX_Brands_EmailId",
                table: "Brands");

            migrationBuilder.DropIndex(
                name: "IX_BrandMembers_BrandId",
                table: "BrandMembers");

            migrationBuilder.DropIndex(
                name: "IX_BrandMembers_EmailId",
                table: "BrandMembers");

            migrationBuilder.DropColumn(
                name: "BrandId",
                table: "Wallets");

            migrationBuilder.DropColumn(
                name: "DistributorId",
                table: "Wallets");

            migrationBuilder.DropColumn(
                name: "EmailId",
                table: "Wallets");

            migrationBuilder.DropColumn(
                name: "EmailId",
                table: "SuspenseWallets");

            migrationBuilder.DropColumn(
                name: "EmailId",
                table: "Distributors");

            migrationBuilder.DropColumn(
                name: "BrandId",
                table: "DistrbuteTransactions");

            migrationBuilder.DropColumn(
                name: "DistributorId",
                table: "DistrbuteTransactions");

            migrationBuilder.DropColumn(
                name: "BrandId",
                table: "Campaign");

            migrationBuilder.DropColumn(
                name: "EmailId",
                table: "Brands");

            migrationBuilder.DropColumn(
                name: "BrandId",
                table: "BrandMembers");

            migrationBuilder.DropColumn(
                name: "EmailId",
                table: "BrandMembers");

            migrationBuilder.AddForeignKey(
                name: "FK_BrandMembers_Brands_Id",
                table: "BrandMembers",
                column: "Id",
                principalTable: "Brands",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BrandMembers_Emails_Id",
                table: "BrandMembers",
                column: "Id",
                principalTable: "Emails",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Brands_Emails_Id",
                table: "Brands",
                column: "Id",
                principalTable: "Emails",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Campaign_Brands_Id",
                table: "Campaign",
                column: "Id",
                principalTable: "Brands",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DistrbuteTransactions_Brands_Id",
                table: "DistrbuteTransactions",
                column: "Id",
                principalTable: "Brands",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DistrbuteTransactions_Distributors_Id",
                table: "DistrbuteTransactions",
                column: "Id",
                principalTable: "Distributors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Distributors_Emails_Id",
                table: "Distributors",
                column: "Id",
                principalTable: "Emails",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SuspenseWallets_Emails_Id",
                table: "SuspenseWallets",
                column: "Id",
                principalTable: "Emails",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Wallets_Brands_Id",
                table: "Wallets",
                column: "Id",
                principalTable: "Brands",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Wallets_Distributors_Id",
                table: "Wallets",
                column: "Id",
                principalTable: "Distributors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Wallets_Emails_Id",
                table: "Wallets",
                column: "Id",
                principalTable: "Emails",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
