using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BetaBank.Migrations
{
    public partial class UserEvents : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_TransactionStatusModels_TransactionStatusModelId",
                table: "Transactions");

            migrationBuilder.DropTable(
                name: "TransactionStatuses");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_TransactionStatusModelId",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "TransactionStatusModelId",
                table: "Transactions");

            migrationBuilder.CreateTable(
                name: "UserEvents",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Action = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Section = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EntityType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    EntityId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserEvents", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserEvents");

            migrationBuilder.AddColumn<string>(
                name: "TransactionStatusModelId",
                table: "Transactions",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "TransactionStatuses",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    SupportId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TransactionId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransactionStatuses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TransactionStatuses_Transactions_SupportId",
                        column: x => x.SupportId,
                        principalTable: "Transactions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TransactionStatuses_TransactionStatusModels_TransactionId",
                        column: x => x.TransactionId,
                        principalTable: "TransactionStatusModels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_TransactionStatusModelId",
                table: "Transactions",
                column: "TransactionStatusModelId");

            migrationBuilder.CreateIndex(
                name: "IX_TransactionStatuses_SupportId",
                table: "TransactionStatuses",
                column: "SupportId");

            migrationBuilder.CreateIndex(
                name: "IX_TransactionStatuses_TransactionId",
                table: "TransactionStatuses",
                column: "TransactionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_TransactionStatusModels_TransactionStatusModelId",
                table: "Transactions",
                column: "TransactionStatusModelId",
                principalTable: "TransactionStatusModels",
                principalColumn: "Id");
        }
    }
}
