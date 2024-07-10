using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BetaBank.Migrations
{
    public partial class Supportenums : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SupportStatusModels",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupportStatusModels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SupportStatuses",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    SupportId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    StatusId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupportStatuses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SupportStatuses_Supports_SupportId",
                        column: x => x.SupportId,
                        principalTable: "Supports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SupportStatuses_SupportStatusModels_StatusId",
                        column: x => x.StatusId,
                        principalTable: "SupportStatusModels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SupportStatuses_StatusId",
                table: "SupportStatuses",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_SupportStatuses_SupportId",
                table: "SupportStatuses",
                column: "SupportId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SupportStatuses");

            migrationBuilder.DropTable(
                name: "SupportStatusModels");
        }
    }
}
