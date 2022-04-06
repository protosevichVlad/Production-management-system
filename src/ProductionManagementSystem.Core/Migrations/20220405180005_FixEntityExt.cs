using Microsoft.EntityFrameworkCore.Migrations;

namespace ProductionManagementSystem.Core.Migrations
{
    public partial class FixEntityExt : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TableColumns_DatabaseTables_TableId",
                table: "TableColumns");

            migrationBuilder.DropColumn(
                name: "DatabaseTableId",
                table: "TableColumns");

            migrationBuilder.AlterColumn<int>(
                name: "TableId",
                table: "TableColumns",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_TableColumns_DatabaseTables_TableId",
                table: "TableColumns",
                column: "TableId",
                principalTable: "DatabaseTables",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TableColumns_DatabaseTables_TableId",
                table: "TableColumns");

            migrationBuilder.AlterColumn<int>(
                name: "TableId",
                table: "TableColumns",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "DatabaseTableId",
                table: "TableColumns",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_TableColumns_DatabaseTables_TableId",
                table: "TableColumns",
                column: "TableId",
                principalTable: "DatabaseTables",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
