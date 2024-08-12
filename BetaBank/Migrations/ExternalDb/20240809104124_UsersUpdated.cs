using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BetaBank.Migrations.ExternalDb
{
    public partial class UsersUpdated : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FatherName",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FatherName",
                table: "Users");
        }
    }
}
