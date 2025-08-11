using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OmniSuite.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class CreateDepositPaymentCode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PaymentCode",
                table: "Deposits",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaymentCode",
                table: "Deposits");
        }
    }
}
