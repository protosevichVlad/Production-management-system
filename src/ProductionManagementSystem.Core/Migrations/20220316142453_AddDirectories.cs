using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ProductionManagementSystem.Core.Migrations
{
    public partial class AddDirectories : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DirectoryId",
                table: "DatabaseTables",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AltiumDB_Directories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    DirectoryName = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ParentId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AltiumDB_Directories", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_DatabaseTables_DirectoryId",
                table: "DatabaseTables",
                column: "DirectoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_DatabaseTables_AltiumDB_Directories_DirectoryId",
                table: "DatabaseTables",
                column: "DirectoryId",
                principalTable: "AltiumDB_Directories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DatabaseTables_AltiumDB_Directories_DirectoryId",
                table: "DatabaseTables");

            migrationBuilder.DropTable(
                name: "AltiumDB_Directories");

            migrationBuilder.DropIndex(
                name: "IX_DatabaseTables_DirectoryId",
                table: "DatabaseTables");

            migrationBuilder.DropColumn(
                name: "DirectoryId",
                table: "DatabaseTables");
        }
    }
}
