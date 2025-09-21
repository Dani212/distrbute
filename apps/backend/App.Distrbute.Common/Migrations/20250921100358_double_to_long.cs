using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace App.Distrbute.Common.Migrations
{
    /// <inheritdoc />
    public partial class double_to_long : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "EstimatedConversions",
                table: "PostMetrics",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "double precision");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "EstimatedConversions",
                table: "PostMetrics",
                type: "double precision",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");
        }
    }
}
