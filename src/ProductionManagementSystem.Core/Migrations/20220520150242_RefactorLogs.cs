using Microsoft.EntityFrameworkCore.Migrations;

namespace ProductionManagementSystem.Core.Migrations
{
    public partial class RefactorLogs : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DesignId",
                table: "Logs");

            migrationBuilder.DropColumn(
                name: "DesignSupplyRequestId",
                table: "Logs");

            migrationBuilder.DropColumn(
                name: "DeviceId",
                table: "Logs");

            migrationBuilder.DropColumn(
                name: "MontageId",
                table: "Logs");

            migrationBuilder.DropColumn(
                name: "MontageSupplyRequestId",
                table: "Logs");

            migrationBuilder.DropColumn(
                name: "OrderId",
                table: "Logs");

            migrationBuilder.DropColumn(
                name: "TaskId",
                table: "Logs");

            migrationBuilder.AddColumn<int>(
                name: "ItemId",
                table: "Logs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ItemType",
                table: "Logs",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ItemId",
                table: "Logs");

            migrationBuilder.DropColumn(
                name: "ItemType",
                table: "Logs");

            migrationBuilder.AddColumn<int>(
                name: "DesignId",
                table: "Logs",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DesignSupplyRequestId",
                table: "Logs",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DeviceId",
                table: "Logs",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MontageId",
                table: "Logs",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MontageSupplyRequestId",
                table: "Logs",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OrderId",
                table: "Logs",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TaskId",
                table: "Logs",
                type: "int",
                nullable: true);
        }
    }
}
