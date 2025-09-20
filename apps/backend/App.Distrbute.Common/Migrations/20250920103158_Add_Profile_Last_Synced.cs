using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace App.Distrbute.Common.Migrations
{
    /// <inheritdoc />
    public partial class Add_Profile_Last_Synced : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LastSynced",
                table: "DistributorSocialAccounts",
                newName: "ProfileLastSynced");

            migrationBuilder.RenameColumn(
                name: "LastSynced",
                table: "BrandSocialAccounts",
                newName: "ProfileLastSynced");

            migrationBuilder.AddColumn<DateTime>(
                name: "PostsLastSynced",
                table: "DistributorSocialAccounts",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "PostsLastSynced",
                table: "BrandSocialAccounts",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PostsLastSynced",
                table: "DistributorSocialAccounts");

            migrationBuilder.DropColumn(
                name: "PostsLastSynced",
                table: "BrandSocialAccounts");

            migrationBuilder.RenameColumn(
                name: "ProfileLastSynced",
                table: "DistributorSocialAccounts",
                newName: "LastSynced");

            migrationBuilder.RenameColumn(
                name: "ProfileLastSynced",
                table: "BrandSocialAccounts",
                newName: "LastSynced");
        }
    }
}
