using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ProductionManagementSystem.Core.Migrations
{
    public partial class AddEntityExt : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DatabaseTables_AltiumDB_Directories_DirectoryId",
                table: "DatabaseTables");

            migrationBuilder.DropForeignKey(
                name: "FK_TableColumns_DatabaseTables_DatabaseTableId",
                table: "TableColumns");

            migrationBuilder.DropTable(
                name: "AltiumDB_Directories");

            migrationBuilder.DropIndex(
                name: "IX_TableColumns_DatabaseTableId",
                table: "TableColumns");

            migrationBuilder.DropIndex(
                name: "IX_DatabaseTables_DirectoryId",
                table: "DatabaseTables");

            migrationBuilder.DropColumn(
                name: "DirectoryId",
                table: "DatabaseTables");

            migrationBuilder.AddColumn<int>(
                name: "TableId",
                table: "TableColumns",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Entities",
                columns: table => new
                {
                    KeyId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    ImageUrl = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PartNumber = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TableId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Entities", x => x.KeyId);
                    table.ForeignKey(
                        name: "FK_Entities_DatabaseTables_TableId",
                        column: x => x.TableId,
                        principalTable: "DatabaseTables",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_TableColumns_TableId",
                table: "TableColumns",
                column: "TableId");

            migrationBuilder.CreateIndex(
                name: "IX_Entities_TableId",
                table: "Entities",
                column: "TableId");

            migrationBuilder.AddForeignKey(
                name: "FK_TableColumns_DatabaseTables_TableId",
                table: "TableColumns",
                column: "TableId",
                principalTable: "DatabaseTables",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TableColumns_DatabaseTables_TableId",
                table: "TableColumns");

            migrationBuilder.DropTable(
                name: "Entities");

            migrationBuilder.DropIndex(
                name: "IX_TableColumns_TableId",
                table: "TableColumns");

            migrationBuilder.DropColumn(
                name: "TableId",
                table: "TableColumns");

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
                name: "IX_TableColumns_DatabaseTableId",
                table: "TableColumns",
                column: "DatabaseTableId");

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

            migrationBuilder.AddForeignKey(
                name: "FK_TableColumns_DatabaseTables_DatabaseTableId",
                table: "TableColumns",
                column: "DatabaseTableId",
                principalTable: "DatabaseTables",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
