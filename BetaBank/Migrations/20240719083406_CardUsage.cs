using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BetaBank.Migrations
{
    public partial class CardUsage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CardUsages",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CardId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UsageDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CardUsages", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CardUsages");
        }
    }
}
