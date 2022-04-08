using Microsoft.EntityFrameworkCore.Migrations;

namespace ProductionManagementSystem.Core.Migrations
{
    public partial class changePartNumberToEntityIdInProjects : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PartNumber",
                table: "AltiumDB_EntityInProjects");

            migrationBuilder.AddColumn<int>(
                name: "EntityId",
                table: "AltiumDB_EntityInProjects",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EntityId",
                table: "AltiumDB_EntityInProjects");

            migrationBuilder.AddColumn<string>(
                name: "PartNumber",
                table: "AltiumDB_EntityInProjects",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }
    }
}
