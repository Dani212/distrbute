using System.Collections.Generic;
using App.Distrbute.Common.Models;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace App.Distrbute.Common.Migrations
{
    /// <inheritdoc />
    public partial class correct_distributor_social_account : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExcludeFromNiche",
                table: "DistributorSocialAccounts");

            migrationBuilder.DropColumn(
                name: "Niches",
                table: "DistributorSocialAccounts");

            migrationBuilder.AddColumn<string>(
                name: "DistributorSocialAccountId",
                table: "DistributorNiches",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DistributorSocialAccountId",
                table: "BrandNiches",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_DistributorNiches_DistributorSocialAccountId",
                table: "DistributorNiches",
                column: "DistributorSocialAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_BrandNiches_DistributorSocialAccountId",
                table: "BrandNiches",
                column: "DistributorSocialAccountId");

            migrationBuilder.AddForeignKey(
                name: "FK_BrandNiches_DistributorSocialAccounts_DistributorSocialAcco~",
                table: "BrandNiches",
                column: "DistributorSocialAccountId",
                principalTable: "DistributorSocialAccounts",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_DistributorNiches_DistributorSocialAccounts_DistributorSoci~",
                table: "DistributorNiches",
                column: "DistributorSocialAccountId",
                principalTable: "DistributorSocialAccounts",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BrandNiches_DistributorSocialAccounts_DistributorSocialAcco~",
                table: "BrandNiches");

            migrationBuilder.DropForeignKey(
                name: "FK_DistributorNiches_DistributorSocialAccounts_DistributorSoci~",
                table: "DistributorNiches");

            migrationBuilder.DropIndex(
                name: "IX_DistributorNiches_DistributorSocialAccountId",
                table: "DistributorNiches");

            migrationBuilder.DropIndex(
                name: "IX_BrandNiches_DistributorSocialAccountId",
                table: "BrandNiches");

            migrationBuilder.DropColumn(
                name: "DistributorSocialAccountId",
                table: "DistributorNiches");

            migrationBuilder.DropColumn(
                name: "DistributorSocialAccountId",
                table: "BrandNiches");

            migrationBuilder.AddColumn<List<BrandNiche>>(
                name: "ExcludeFromNiche",
                table: "DistributorSocialAccounts",
                type: "jsonb",
                nullable: false);

            migrationBuilder.AddColumn<List<DistributorNiche>>(
                name: "Niches",
                table: "DistributorSocialAccounts",
                type: "jsonb",
                nullable: false);
        }
    }
}
