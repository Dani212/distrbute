using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace App.Distrbute.Common.Migrations
{
    /// <inheritdoc />
    public partial class add_distributor_niche : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Niches",
                table: "Brands");

            migrationBuilder.CreateTable(
                name: "BrandNiches",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    BrandId = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BrandNiches", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BrandNiches_Brands_BrandId",
                        column: x => x.BrandId,
                        principalTable: "Brands",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "DistributorNiches",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DistributorNiches", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BrandDistributorNicheCorrelations",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    BrandNicheId = table.Column<string>(type: "text", nullable: false),
                    DistributorNicheId = table.Column<string>(type: "text", nullable: false),
                    Weight = table.Column<int>(type: "integer", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BrandDistributorNicheCorrelations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BrandDistributorNicheCorrelations_BrandNiches_BrandNicheId",
                        column: x => x.BrandNicheId,
                        principalTable: "BrandNiches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BrandDistributorNicheCorrelations_DistributorNiches_Distrib~",
                        column: x => x.DistributorNicheId,
                        principalTable: "DistributorNiches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BrandDistributorNicheCorrelations_BrandNicheId",
                table: "BrandDistributorNicheCorrelations",
                column: "BrandNicheId");

            migrationBuilder.CreateIndex(
                name: "IX_BrandDistributorNicheCorrelations_DistributorNicheId",
                table: "BrandDistributorNicheCorrelations",
                column: "DistributorNicheId");

            migrationBuilder.CreateIndex(
                name: "IX_BrandNiches_BrandId",
                table: "BrandNiches",
                column: "BrandId");

            migrationBuilder.CreateIndex(
                name: "IX_BrandNiches_Name",
                table: "BrandNiches",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DistributorNiches_Name",
                table: "DistributorNiches",
                column: "Name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BrandDistributorNicheCorrelations");

            migrationBuilder.DropTable(
                name: "BrandNiches");

            migrationBuilder.DropTable(
                name: "DistributorNiches");

            migrationBuilder.AddColumn<string>(
                name: "Niches",
                table: "Brands",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
