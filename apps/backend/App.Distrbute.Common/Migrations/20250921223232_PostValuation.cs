using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace App.Distrbute.Common.Migrations
{
    /// <inheritdoc />
    public partial class PostValuation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Posts_PostMetrics_PostValuationId",
                table: "Posts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PostMetrics",
                table: "PostMetrics");

            migrationBuilder.RenameTable(
                name: "PostMetrics",
                newName: "PostValuations");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PostValuations",
                table: "PostValuations",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_PostValuations_PostValuationId",
                table: "Posts",
                column: "PostValuationId",
                principalTable: "PostValuations",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Posts_PostValuations_PostValuationId",
                table: "Posts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PostValuations",
                table: "PostValuations");

            migrationBuilder.RenameTable(
                name: "PostValuations",
                newName: "PostMetrics");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PostMetrics",
                table: "PostMetrics",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_PostMetrics_PostValuationId",
                table: "Posts",
                column: "PostValuationId",
                principalTable: "PostMetrics",
                principalColumn: "Id");
        }
    }
}
