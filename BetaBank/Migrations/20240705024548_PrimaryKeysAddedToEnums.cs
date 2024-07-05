using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BetaBank.Migrations
{
    public partial class PrimaryKeysAddedToEnums : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "TransactionStatuses",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "TransactionCardTypes",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "BankCardTypes",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "BankCardStatuses",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TransactionStatuses",
                table: "TransactionStatuses",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TransactionCardTypes",
                table: "TransactionCardTypes",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BankCardTypes",
                table: "BankCardTypes",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BankCardStatuses",
                table: "BankCardStatuses",
                column: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_TransactionStatuses",
                table: "TransactionStatuses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TransactionCardTypes",
                table: "TransactionCardTypes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BankCardTypes",
                table: "BankCardTypes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BankCardStatuses",
                table: "BankCardStatuses");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "TransactionStatuses");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "TransactionCardTypes");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "BankCardTypes");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "BankCardStatuses");
        }
    }
}
