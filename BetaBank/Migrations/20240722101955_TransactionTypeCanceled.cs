using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BetaBank.Migrations
{
    public partial class TransactionTypeCanceled : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TransactionTypes");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TransactionTypes",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TransactionId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TypeId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransactionTypes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TransactionTypes_Transactions_TransactionId",
                        column: x => x.TransactionId,
                        principalTable: "Transactions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TransactionTypes_TransactionTypeModels_TypeId",
                        column: x => x.TypeId,
                        principalTable: "TransactionTypeModels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TransactionTypes_TransactionId",
                table: "TransactionTypes",
                column: "TransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_TransactionTypes_TypeId",
                table: "TransactionTypes",
                column: "TypeId");
        }
    }
}
