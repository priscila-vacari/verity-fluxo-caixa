using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FluxoCaixa.Infra.Migrations
{
    /// <inheritdoc />
    [ExcludeFromCodeCoverage]
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "consolidations",
                columns: table => new
                {
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    total_credit = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    total_dedit = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    balance = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_consolidations", x => x.Date);
                });

            migrationBuilder.CreateTable(
                name: "launches",
                columns: table => new
                {
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_launches", x => new { x.Date, x.Type });
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "consolidations");

            migrationBuilder.DropTable(
                name: "launches");
        }
    }
}
