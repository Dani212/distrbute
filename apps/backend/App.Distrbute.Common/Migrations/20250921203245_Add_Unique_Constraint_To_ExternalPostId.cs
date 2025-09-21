using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace App.Distrbute.Common.Migrations
{
    /// <inheritdoc />
    public partial class Add_Unique_Constraint_To_ExternalPostId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Posts_ExternalPostId",
                table: "Posts",
                column: "ExternalPostId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Posts_ExternalPostId",
                table: "Posts");
        }
    }
}
