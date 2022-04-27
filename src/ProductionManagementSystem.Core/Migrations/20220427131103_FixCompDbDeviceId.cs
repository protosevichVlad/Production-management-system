using Microsoft.EntityFrameworkCore.Migrations;

namespace ProductionManagementSystem.Core.Migrations
{
    public partial class FixCompDbDeviceId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UsedItems_CompDB_Devices_CompDbDeviceId",
                table: "UsedItems");

            migrationBuilder.DropIndex(
                name: "IX_UsedItems_CompDbDeviceId",
                table: "UsedItems");

            migrationBuilder.DropColumn(
                name: "CompDbDeviceId",
                table: "UsedItems");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CompDbDeviceId",
                table: "UsedItems",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UsedItems_CompDbDeviceId",
                table: "UsedItems",
                column: "CompDbDeviceId");

            migrationBuilder.AddForeignKey(
                name: "FK_UsedItems_CompDB_Devices_CompDbDeviceId",
                table: "UsedItems",
                column: "CompDbDeviceId",
                principalTable: "CompDB_Devices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
