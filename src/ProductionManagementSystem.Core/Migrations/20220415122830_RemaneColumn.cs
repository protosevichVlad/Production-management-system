using Microsoft.EntityFrameworkCore.Migrations;

namespace ProductionManagementSystem.Core.Migrations
{
    public partial class RemaneColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UsedInDevice_CompDB_Devices_CompDbDeviceId",
                table: "UsedInDevice");

            migrationBuilder.AlterColumn<int>(
                name: "CompDbDeviceId",
                table: "UsedInDevice",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_UsedInDevice_CompDB_Devices_CompDbDeviceId",
                table: "UsedInDevice",
                column: "CompDbDeviceId",
                principalTable: "CompDB_Devices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UsedInDevice_CompDB_Devices_CompDbDeviceId",
                table: "UsedInDevice");

            migrationBuilder.AlterColumn<int>(
                name: "CompDbDeviceId",
                table: "UsedInDevice",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_UsedInDevice_CompDB_Devices_CompDbDeviceId",
                table: "UsedInDevice",
                column: "CompDbDeviceId",
                principalTable: "CompDB_Devices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
