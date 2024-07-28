using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BetaBank.Migrations
{
    public partial class PaymentsModelsAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Icon",
                table: "Transactions",
                newName: "Title");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Transactions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "InternetModels",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsAppointmentTypeable = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InternetModels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MobileOperatorModels",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MobileOperatorModels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UtilityModels",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsAppointmentTypeable = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UtilityModels", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InternetModels");

            migrationBuilder.DropTable(
                name: "MobileOperatorModels");

            migrationBuilder.DropTable(
                name: "UtilityModels");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Transactions");

            migrationBuilder.RenameColumn(
                name: "Title",
                table: "Transactions",
                newName: "Icon");
        }
    }
}
