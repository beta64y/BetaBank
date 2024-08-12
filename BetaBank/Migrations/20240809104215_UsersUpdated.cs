using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BetaBank.Migrations
{
    public partial class UsersUpdated : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FatherName",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FatherName",
                table: "AspNetUsers");
        }
    }
}
