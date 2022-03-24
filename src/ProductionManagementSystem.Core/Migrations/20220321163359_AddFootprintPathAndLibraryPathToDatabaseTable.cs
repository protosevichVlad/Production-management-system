using Microsoft.EntityFrameworkCore.Migrations;

namespace ProductionManagementSystem.Core.Migrations
{
    public partial class AddFootprintPathAndLibraryPathToDatabaseTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FootprintPath",
                table: "DatabaseTables",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "LibraryPath",
                table: "DatabaseTables",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FootprintPath",
                table: "DatabaseTables");

            migrationBuilder.DropColumn(
                name: "LibraryPath",
                table: "DatabaseTables");
        }
    }
}
