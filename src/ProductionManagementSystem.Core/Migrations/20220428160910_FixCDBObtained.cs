using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ProductionManagementSystem.Core.Migrations
{
    public partial class FixCDBObtained : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ObtainedId",
                table: "CDBObtained",
                newName: "UsedItemId");

            migrationBuilder.AlterColumn<DateTime>(
                name: "EndDate",
                table: "CDBTasks",
                type: "datetime(6)",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)");

            migrationBuilder.CreateIndex(
                name: "IX_CDBObtained_UsedItemId",
                table: "CDBObtained",
                column: "UsedItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_CDBObtained_UsedItems_UsedItemId",
                table: "CDBObtained",
                column: "UsedItemId",
                principalTable: "UsedItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CDBObtained_UsedItems_UsedItemId",
                table: "CDBObtained");

            migrationBuilder.DropIndex(
                name: "IX_CDBObtained_UsedItemId",
                table: "CDBObtained");

            migrationBuilder.RenameColumn(
                name: "UsedItemId",
                table: "CDBObtained",
                newName: "ObtainedId");

            migrationBuilder.AlterColumn<DateTime>(
                name: "EndDate",
                table: "CDBTasks",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldNullable: true);
        }
    }
}
