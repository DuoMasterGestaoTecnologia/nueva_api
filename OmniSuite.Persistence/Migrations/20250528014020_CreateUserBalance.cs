using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OmniSuite.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class CreateUserBalance : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Deposits_Users_UserId",
                table: "Deposits");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Deposits",
                table: "Deposits");

            migrationBuilder.RenameTable(
                name: "Deposits",
                newName: "UserBalances");

            migrationBuilder.RenameIndex(
                name: "IX_Deposits_UserId",
                table: "UserBalances",
                newName: "IX_UserBalances_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserBalances",
                table: "UserBalances",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "UserBalance",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    UserId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    TotalAmount = table.Column<long>(type: "bigint", nullable: false),
                    TotalBlocked = table.Column<long>(type: "bigint", nullable: true),
                    TotalPending = table.Column<long>(type: "bigint", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserBalance", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

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
                name: "UserBalance");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserBalances",
                table: "UserBalances");

            migrationBuilder.RenameTable(
                name: "UserBalances",
                newName: "Deposits");

            migrationBuilder.RenameIndex(
                name: "IX_UserBalances_UserId",
                table: "Deposits",
                newName: "IX_Deposits_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Deposits",
                table: "Deposits",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Deposits_Users_UserId",
                table: "Deposits",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
