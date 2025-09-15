using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OmniSuite.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddDigitalProductCategories : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Category",
                table: "DigitalProducts");

            migrationBuilder.AddColumn<Guid>(
                name: "CategoryId",
                table: "DigitalProducts",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.CreateTable(
                name: "DigitalProductCategories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IconUrl = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Color = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DigitalProductCategories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DigitalProductCategories_Users_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_DigitalProducts_CategoryId",
                table: "DigitalProducts",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_DigitalProductCategories_CreatedBy",
                table: "DigitalProductCategories",
                column: "CreatedBy");

            migrationBuilder.AddForeignKey(
                name: "FK_DigitalProducts_DigitalProductCategories_CategoryId",
                table: "DigitalProducts",
                column: "CategoryId",
                principalTable: "DigitalProductCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DigitalProducts_DigitalProductCategories_CategoryId",
                table: "DigitalProducts");

            migrationBuilder.DropTable(
                name: "DigitalProductCategories");

            migrationBuilder.DropIndex(
                name: "IX_DigitalProducts_CategoryId",
                table: "DigitalProducts");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "DigitalProducts");

            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "DigitalProducts",
                type: "varchar(100)",
                maxLength: 100,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }
    }
}
