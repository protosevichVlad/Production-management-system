using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ProductionManagementSystem.Core.Migrations
{
    public partial class AddEntityExt : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TableColumns_DatabaseTables_DatabaseTableId",
                table: "TableColumns");

            migrationBuilder.DropTable(
                name: "DatabaseTables");

            migrationBuilder.DropTable(
                name: "AltiumDB_Directories");

            migrationBuilder.DropColumn(
                name: "PartNumber",
                table: "AltiumDB_EntityInProjects");

            migrationBuilder.RenameColumn(
                name: "DatabaseTableId",
                table: "TableColumns",
                newName: "TableId");

            migrationBuilder.RenameIndex(
                name: "IX_TableColumns_DatabaseTableId",
                table: "TableColumns",
                newName: "IX_TableColumns_TableId");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "AltiumDB_Projects",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "Quantity",
                table: "AltiumDB_Projects",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "EntityId",
                table: "AltiumDB_EntityInProjects",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Tables",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    TableName = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DisplayName = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FootprintPath = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    LibraryPath = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tables", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

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
                        name: "FK_Entities_Tables_TableId",
                        column: x => x.TableId,
                        principalTable: "Tables",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Entities_TableId",
                table: "Entities",
                column: "TableId");

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
                name: "FK_TableColumns_Tables_TableId",
                table: "TableColumns");

            migrationBuilder.DropTable(
                name: "Entities");

            migrationBuilder.DropTable(
                name: "Tables");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "AltiumDB_Projects");

            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "AltiumDB_Projects");

            migrationBuilder.DropColumn(
                name: "EntityId",
                table: "AltiumDB_EntityInProjects");

            migrationBuilder.RenameColumn(
                name: "TableId",
                table: "TableColumns",
                newName: "DatabaseTableId");

            migrationBuilder.RenameIndex(
                name: "IX_TableColumns_TableId",
                table: "TableColumns",
                newName: "IX_TableColumns_DatabaseTableId");

            migrationBuilder.AddColumn<string>(
                name: "PartNumber",
                table: "AltiumDB_EntityInProjects",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

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

            migrationBuilder.CreateTable(
                name: "DatabaseTables",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    DirectoryId = table.Column<int>(type: "int", nullable: true),
                    DisplayName = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FootprintPath = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    LibraryPath = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TableName = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DatabaseTables", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DatabaseTables_AltiumDB_Directories_DirectoryId",
                        column: x => x.DirectoryId,
                        principalTable: "AltiumDB_Directories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_DatabaseTables_DirectoryId",
                table: "DatabaseTables",
                column: "DirectoryId");

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
