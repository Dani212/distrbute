using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace App.Distrbute.Common.Migrations
{
    /// <inheritdoc />
    public partial class fix_brand_not_nullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Posts_Brands_BrandId",
                table: "Posts");

            migrationBuilder.AlterColumn<string>(
                name: "BrandId",
                table: "Posts",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_Brands_BrandId",
                table: "Posts",
                column: "BrandId",
                principalTable: "Brands",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Posts_Brands_BrandId",
                table: "Posts");

            migrationBuilder.AlterColumn<string>(
                name: "BrandId",
                table: "Posts",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_Brands_BrandId",
                table: "Posts",
                column: "BrandId",
                principalTable: "Brands",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
