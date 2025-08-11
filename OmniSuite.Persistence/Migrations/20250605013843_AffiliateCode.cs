using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OmniSuite.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AffiliateCode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AffiliateCode",
                table: "Affiliates",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AffiliateCode",
                table: "Affiliates");
        }
    }
}
