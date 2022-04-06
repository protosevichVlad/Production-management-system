using Microsoft.EntityFrameworkCore.Migrations;

namespace ProductionManagementSystem.Core.Migrations
{
    public partial class FixEntityExt2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Entities_DatabaseTables_TableId",
                table: "Entities");

            migrationBuilder.DropForeignKey(
                name: "FK_TableColumns_DatabaseTables_TableId",
                table: "TableColumns");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DatabaseTables",
                table: "DatabaseTables");

            migrationBuilder.RenameTable(
                name: "DatabaseTables",
                newName: "Tables");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Tables",
                table: "Tables",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Entities_Tables_TableId",
                table: "Entities",
                column: "TableId",
                principalTable: "Tables",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TableColumns_Tables_TableId",
                table: "TableColumns",
                column: "TableId",
                principalTable: "Tables",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Entities_Tables_TableId",
                table: "Entities");

            migrationBuilder.DropForeignKey(
                name: "FK_TableColumns_Tables_TableId",
                table: "TableColumns");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Tables",
                table: "Tables");

            migrationBuilder.RenameTable(
                name: "Tables",
                newName: "DatabaseTables");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DatabaseTables",
                table: "DatabaseTables",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Entities_DatabaseTables_TableId",
                table: "Entities",
                column: "TableId",
                principalTable: "DatabaseTables",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TableColumns_DatabaseTables_TableId",
                table: "TableColumns",
                column: "TableId",
                principalTable: "DatabaseTables",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
