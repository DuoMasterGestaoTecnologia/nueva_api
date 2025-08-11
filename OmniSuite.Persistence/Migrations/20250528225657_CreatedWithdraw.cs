using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OmniSuite.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class CreatedWithdraw : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Document",
                table: "Users",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Withdraw",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    UserId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Amount = table.Column<long>(type: "bigint", nullable: false),
                    TransactionStatus = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    ExternalId = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Withdraw", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Withdraw_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_UserBalances_UserId",
                table: "UserBalances",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Withdraw_UserId",
                table: "Withdraw",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserBalances_Users_UserId",
                table: "UserBalances",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserBalances_Users_UserId",
                table: "UserBalances");

            migrationBuilder.DropTable(
                name: "Withdraw");

            migrationBuilder.DropIndex(
                name: "IX_UserBalances_UserId",
                table: "UserBalances");

            migrationBuilder.DropColumn(
                name: "Document",
                table: "Users");
        }
    }
}
