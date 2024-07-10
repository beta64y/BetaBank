using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BetaBank.Migrations
{
    public partial class imagesupdate1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Image",
                table: "News",
                newName: "SecondImage");

            migrationBuilder.AddColumn<string>(
                name: "FirstImage",
                table: "News",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FirstImage",
                table: "News");

            migrationBuilder.RenameColumn(
                name: "SecondImage",
                table: "News",
                newName: "Image");
        }
    }
}
