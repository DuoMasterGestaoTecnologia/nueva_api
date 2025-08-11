using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OmniSuite.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class CreateUserBalanceCorrectly : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserBalances_Users_UserId",
                table: "UserBalances");

            migrationBuilder.DropTable(
                name: "UserBalance");

            migrationBuilder.DropIndex(
                name: "IX_UserBalances_UserId",
                table: "UserBalances");

            migrationBuilder.DropColumn(
                name: "ExternalId",
                table: "UserBalances");

            migrationBuilder.DropColumn(
                name: "PaymentCode",
                table: "UserBalances");

            migrationBuilder.DropColumn(
                name: "PaymentMethod",
                table: "UserBalances");

            migrationBuilder.DropColumn(
                name: "TransactionStatus",
                table: "UserBalances");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "UserBalances",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "Amount",
                table: "UserBalances",
                newName: "TotalAmount");

            migrationBuilder.AddColumn<long>(
                name: "TotalBlocked",
                table: "UserBalances",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "TotalPending",
                table: "UserBalances",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Deposits",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    UserId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Amount = table.Column<long>(type: "bigint", nullable: false),
                    PaymentMethod = table.Column<int>(type: "int", nullable: false),
                    TransactionStatus = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    PaymentCode = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ExternalId = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Deposits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Deposits_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Deposits_UserId",
                table: "Deposits",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Deposits");

            migrationBuilder.DropColumn(
                name: "TotalBlocked",
                table: "UserBalances");

            migrationBuilder.DropColumn(
                name: "TotalPending",
                table: "UserBalances");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "UserBalances",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "TotalAmount",
                table: "UserBalances",
                newName: "Amount");

            migrationBuilder.AddColumn<string>(
                name: "ExternalId",
                table: "UserBalances",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "PaymentCode",
                table: "UserBalances",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "PaymentMethod",
                table: "UserBalances",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TransactionStatus",
                table: "UserBalances",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "UserBalance",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    TotalAmount = table.Column<long>(type: "bigint", nullable: false),
                    TotalBlocked = table.Column<long>(type: "bigint", nullable: true),
                    TotalPending = table.Column<long>(type: "bigint", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UserId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserBalance", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_UserBalances_UserId",
                table: "UserBalances",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserBalances_Users_UserId",
                table: "UserBalances",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
